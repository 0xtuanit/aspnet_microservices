using Infrastructure.Configurations;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ordering.API.Application.IntegrationEvents.EventsHandler;
using Shared.Configurations;

namespace Ordering.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
            .Get<DatabaseSettings>();
        if (databaseSettings != null) services.AddSingleton(databaseSettings);

        var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
            .Get<SMTPEmailSetting>();
        if (emailSettings != null) services.AddSingleton(emailSettings);

        return services;
    }

    public static void ConfigureMassTransit(this IServiceCollection services)
    {
        var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
        if (settings == null || string.IsNullOrEmpty(settings.HostAddress))
            throw new ArgumentNullException($"{nameof(EventBusSettings)} is not configured.");

        var mqConnection = new Uri(settings.HostAddress);
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(config =>
        {
            config.AddConsumersFromNamespaceContaining<BasketCheckoutEventHandler>();
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(mqConnection);

                // cfg.ReceiveEndpoint("basket-checkout-queue", c =>
                // {
                //     c.ConfigureConsumer<BasketCheckoutEventHandler>(ctx);
                // });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    }

    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        services.AddHealthChecks()
            .AddSqlServer(databaseSettings.ConnectionString,
                name: "SqlServer Health",
                failureStatus: HealthStatus.Degraded);
    }
}