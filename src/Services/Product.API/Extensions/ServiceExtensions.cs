using System.Text;
using Contracts.Domains.Interfaces;
using Contracts.Identity;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;
using Shared.Configurations;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    // internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     var jwtSettings = configuration.GetSection(nameof(JwtSettings))
    //         .Get<JwtSettings>();
    //     services.AddSingleton(jwtSettings);
    //
    //     return services;
    // }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureProductDbContext(configuration);
        services.AddInfrastructureServices();
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

        return services;
    }

    private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        var builder = new MySqlConnectionStringBuilder(connectionString!);

        services.AddDbContext<ProductContext>(m => m.UseMySql(builder.ConnectionString,
            ServerVersion.AutoDetect(builder.ConnectionString),
            e =>
            {
                e.MigrationsAssembly("Product.API");
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
            .AddScoped(serviceType: typeof(IUnitOfWork<>), implementationType: typeof(UnitOfWork<>))
            .AddScoped<IProductRepository, ProductRepository>()
            .AddTransient<ErrorWrappingMiddleware>();
    }
}