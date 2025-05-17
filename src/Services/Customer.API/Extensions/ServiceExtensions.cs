using Contracts.Domains.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Middlewares;
using Microsoft.EntityFrameworkCore;
using Shared.Configurations;

namespace Customer.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
            .Get<DatabaseSettings>();
        if (databaseSettings != null) services.AddSingleton(databaseSettings);

        var hangFireSettings = configuration.GetSection(nameof(HangfireSettings))
            .Get<HangfireSettings>();
        if (hangFireSettings != null) services.AddSingleton(hangFireSettings);

        return services;
    }

    public static void ConfigureCustomerContext(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
            throw new ArgumentNullException($"{nameof(DatabaseSettings)} is not configured.");

        services.AddDbContext<CustomerContext>(options => options.UseNpgsql(databaseSettings.ConnectionString));
    }

    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        // We have to configure all these for declaring ICustomer repo & ICustomer service
        services.AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped(serviceType: typeof(IUnitOfWork<>), implementationType: typeof(UnitOfWork<>))
            .AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>));
        // .AddTransient<ErrorWrappingMiddleware>();
    }
}