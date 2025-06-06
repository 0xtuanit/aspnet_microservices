using Common.Logging;
using HealthChecks.UI.Client;
using Inventory.Grpc.Extensions;
using Inventory.Grpc.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.ConfigureMongoDbClient();
    builder.Services.AddInfrastructureServices();
    builder.Services.ConfigureHealthChecks();
    builder.Services.AddGrpc();

    // builder.WebHost.ConfigureKestrel(options =>
    // {
    //     // Setup a HTTP/2 endpoint without TLS.
    //     options.ListenLocalhost(5007, o => o.Protocols =
    //         HttpProtocols.Http2);
    // });

    var app = builder.Build();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        // Health checks
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        endpoints.MapGrpcHealthChecksService();

        // Configure the HTTP request pipeline.
        endpoints.MapGrpcService<InventoryService>();
        endpoints.MapGet("/",
            async context =>
            {
                await context.Response.WriteAsync(
                    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
    });

    app.Run();
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