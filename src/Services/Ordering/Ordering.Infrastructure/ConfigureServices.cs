using Contracts.Domains.Interfaces;
using Contracts.Services;
using Infrastructure.Common;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common.Interfaces;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;

namespace Ordering.Infrastructure;

// Inject all necessary services of Infrastructure
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OrderContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"),
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