using Grpc.HealthCheck;
using Infrastructure.Extensions;
using Inventory.Grpc.Repositories;
using Inventory.Grpc.Repositories.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Grpc.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(MongoDbSettings))
            .Get<MongoDbSettings>();
        if (databaseSettings != null) services.AddSingleton(databaseSettings);

        return services;
    }

    private static string GetMongoConnectionString(this IServiceCollection services)
    {
        var settings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings));
        if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException($"{nameof(MongoDbSettings)} is not configured.");

        var databaseName = settings.DatabaseName;
        var mongoDbConnectionString = settings.ConnectionString + "/" + databaseName + "?authSource=admin";

        return mongoDbConnectionString;
    }

    // Config Mongo Client to be able to connect to MongoDB
    public static void ConfigureMongoDbClient(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(
                new MongoClient(GetMongoConnectionString(services)))
            .AddScoped(x => x.GetService<IMongoClient>()?.StartSession());
    }

    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryRepository, InventoryRepository>();
    }

    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings));
        services.AddSingleton<HealthServiceImpl>();
        services.AddHostedService<StatusService>();
        services
            .AddSingleton(sp => new MongoClient(databaseSettings.ConnectionString))
            .AddHealthChecks()
            .AddMongoDb(
                name: "Inventory MongoDb Health",
                failureStatus: HealthStatus.Degraded)
            .AddCheck("Inventory gRPC Health", () => HealthCheckResult.Healthy());
    }
}