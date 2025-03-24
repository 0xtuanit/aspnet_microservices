using Contracts.Domains.Interfaces;
using Contracts.Services;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common.Interfaces;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;
using Shared.Configurations;

namespace Ordering.Infrastructure;

// Inject all necessary services of Infrastructure
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
            throw new ArgumentNullException($"{nameof(DatabaseSettings)} is not configured.");

        services.AddDbContext<OrderContext>(options =>
        {
            options.UseSqlServer(databaseSettings.ConnectionString,
                builder => builder.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
        });

        // Register 'OrderContextSeed' into 'services'
        services.AddScoped<OrderContextSeed>();

        // Inject OrderRepository
        services.AddScoped<IOrderRepository, OrderRepository>();

        // This OrderRepository uses IUnitOfWork, so we also need to inject it
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

        services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));

        return services;
    }
}