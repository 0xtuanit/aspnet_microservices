using Common.Logging;
using Product.API.Extensions;
using Product.API.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

// Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Configuration.AddAppConfigurations(builder.Environment);

    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();
    app.UseInfrastructure();

    app.MigrateDatabase<ProductContext>((context, _) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    }).Run();

    // app.Run();
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