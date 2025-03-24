using Contracts.Configurations;
using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.ScheduledJobs;
using Infrastructure.Services;
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

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddTransient<IScheduledJobService, HangfireService>()
            .AddScoped(typeof(IEmailSMTPSettings), typeof(SMTPEmailSetting))
            .AddScoped<ISmtpEmailService, SmtpEmailService>()
            .AddTransient<IBackgroundJobService, BackgroundJobService>();
}