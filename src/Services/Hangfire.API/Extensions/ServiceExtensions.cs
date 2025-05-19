using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.ScheduledJobs;
using Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Shared.Configurations;

namespace Hangfire.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var hangFireSettings = configuration.GetSection(nameof(HangfireSettings))
            .Get<HangfireSettings>();
        if (hangFireSettings != null) services.AddSingleton(hangFireSettings);

        var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
            .Get<SMTPEmailSetting>();
        if (emailSettings != null) services.AddSingleton(emailSettings);

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddTransient<IScheduledJobService, HangfireService>()
            .AddScoped<ISmtpEmailService, SmtpEmailService>()
            .AddTransient<IBackgroundJobService, BackgroundJobService>();

    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOptions<HangfireSettings>(nameof(HangfireSettings));
        services
            .AddSingleton(sp => new MongoClient(databaseSettings.Storage?.ConnectionString))
            .AddHealthChecks()
            .AddMongoDb(
                name: "MongoDb Health",
                failureStatus: HealthStatus.Degraded);
    }
}