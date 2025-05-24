using Common.Logging;
using Customer.API;
using Customer.API.Controllers;
using Customer.API.Extensions;
using Customer.API.Persistence;
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

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

    builder.Services.ConfigureCustomerContext();
    builder.Services.AddInfrastructureServices();
    builder.Services.AddIonHangfireService();
    builder.Services.ConfigureHealthChecks();

    var app = builder.Build();

    // Map URL following minimal API style
    app.MapGet("/", () => $"Welcome to {builder.Environment.ApplicationName}!");

    app.MapCustomersApi();

    // Looks the same as MapPost above.
    // But will use DTO, not directly use Entities (=> no need to use all fields of Entities)
    // app.MapPut("/api/customers/{id}", async () =>
    // {
    //     
    // });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
                $"{builder.Environment.ApplicationName} v1"));
        });
    }

    // app.UseHttpsRedirection(); //production only
    app.UseRouting();
    app.UseAuthorization();

    app.UseHangfireDashboard(builder.Configuration);

    // app.MapControllers();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapDefaultControllerRoute();
    });

    app.SeedCustomerData()
        .Run();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}