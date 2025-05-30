using Hangfire;
using Shared.Configurations;

namespace Customer.API.Extensions;

public static class HostExtensions
{
    internal static void AddAppConfigurations(this ConfigurationManager config, IWebHostEnvironment env)
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    internal static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
    {
        var configDashboard = configuration.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>();
        var hangfireSettings = configuration.GetSection("HangfireSettings").Get<HangfireSettings>();
        var hangfireRoute = hangfireSettings?.Route;

        app.UseHangfireDashboard(hangfireRoute, new DashboardOptions
        {
            // Authorization = new[] { new HangfireAuthorizationFilter() },
            DashboardTitle = configDashboard?.DashboardTitle,
            StatsPollingInterval = configDashboard!.StatsPollingInterval,
            AppPath = configDashboard.AppPath,
            IgnoreAntiforgeryToken = true
        });

        return app;
    }
}