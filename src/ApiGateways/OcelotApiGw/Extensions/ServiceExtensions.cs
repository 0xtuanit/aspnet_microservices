using System.Text;
using Contracts.Identity;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Shared.Configurations;

namespace OcelotApiGw.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings))
            .Get<JwtSettings>();
        if (jwtSettings != null) services.AddSingleton(jwtSettings);
        return services;
    }

    public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration)
            .AddPolly()
            .AddCacheManager(x => { x.WithDictionaryHandle(); });
        // services.AddSingleton<IOcelotCache<CachedResponse>, IonCached>(); // Our own class if we want to override
        services.AddTransient<ITokenService, TokenService>();
        // services.AddJwtAuthentication();
        services.AddSwaggerForOcelot(configuration, x => { x.GenerateDocsForGatewayItSelf = false; });
    }

    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["AllowOrigins"];
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                if (origins != null)
                    builder.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
            });
        });
    }

    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
        if (settings == null || string.IsNullOrEmpty(settings.Key))
            throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured properly");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = false
        };
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
        {
            x.SaveToken = true;
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = tokenValidationParameters;
        });

        return services;
    }
}