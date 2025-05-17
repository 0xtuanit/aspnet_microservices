using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Interfaces;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Middlewares;
using Inventory.Grpc.Client;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;

namespace Basket.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
            IConfiguration configuration)
        {
            var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();
            if (eventBusSettings != null) services.AddSingleton(eventBusSettings);

            var cacheSettings = configuration.GetSection(nameof(CacheSettings))
                .Get<CacheSettings>();
            if (cacheSettings != null) services.AddSingleton(cacheSettings);

            var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
                .Get<GrpcSettings>();
            if (grpcSettings != null) services.AddSingleton(grpcSettings);

            var backgroundJobSettings = configuration.GetSection(nameof(BackgroundJobSettings))
                .Get<BackgroundJobSettings>();
            if (backgroundJobSettings != null) services.AddSingleton(backgroundJobSettings);

            return services;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
            services.AddScoped<IBasketRepository, BasketRepository>()
                .AddTransient<ISerializeService, SerializeService>()
                .AddTransient<IEmailTemplateService, BasketEmailTemplateService>();
        // .AddTransient<ErrorWrappingMiddleware>();

        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
            if (string.IsNullOrEmpty(settings.ConnectionString))
                throw new ArgumentNullException($"Redis {nameof(CacheSettings)} is not configured");

            //Redis Configuration
            services.AddStackExchangeRedisCache(options => { options.Configuration = settings.ConnectionString; });
        }

        public static void ConfigureHttpClientService(this IServiceCollection services)
        {
            services.AddHttpClient<BackgroundJobHttpService>();
        }

        public static IServiceCollection ConfigureGrpcServices(this IServiceCollection services)
        {
            var settings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
            services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(x =>
            {
                if (settings.StockUrl != null) x.Address = new Uri(settings.StockUrl);
            });
            services.AddScoped<StockItemGrpcService>();

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
                config.UsingRabbitMq((
                    ctx,
                    cfg) =>
                {
                    cfg.Host(mqConnection);
                });

                // Publish submit order message
                config.AddRequestClient<IBasketCheckoutEvent>();
            });
        }
    }
}