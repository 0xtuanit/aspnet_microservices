using Common.Logging;
using Hangfire.API.Extensions;
using HealthChecks.UI.Client;
using Infrastructure.ScheduledJobs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

// Log.Logger = new LoggerConfiguration()
//     .WriteTo.Console()
//     .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Configuration.AddAppConfigurations(builder.Environment);
    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddControllers();
    // builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddIonHangfireService();
    builder.Services.ConfigureServices();
    builder.Services.ConfigureHealthChecks();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
                $"{builder.Environment.ApplicationName} v1"));
    }

    app.UseRouting();
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseHangfireDashboard(builder.Configuration);

    // app.UseEndpoints(ep => { ep.MapDefaultControllerRoute(); });
    // app.MapDefaultControllerRoute(); // Replace the above line of UseEndpoints with this line

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute(); // Automatically adding Home index into Url
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    });

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}