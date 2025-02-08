using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API;
using Customer.API.Controllers;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    // Add services to the container.
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(
        options => options.UseNpgsql(connectionString)
    );

    // We have to configure all these for declaring ICustomer repo & ICustomer service
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
        .AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>))
        .AddScoped<ICustomerService, CustomerService>();

    var app = builder.Build();

    // Map URL following minimal API style
    app.MapGet("/", () => $"Welcome to {builder.Environment.ApplicationName}!");
    // app.MapGet("/api/customers",
    //     async (ICustomerService customerService) => await customerService.GetCustomersAsync());

    app.MapCustomersApi();

    // app.MapPost("/api/customers",
    //     async (Customer.API.Entities.Customer customer, ICustomerRepository customerRepository) =>
    //     {
    //         customerRepository.CreateAsync(customer);
    //         customerRepository.SaveChangesAsync();
    //     });
    //
    // app.MapDelete("/api/customers/{id}", async (int id, ICustomerRepository customerRepository) =>
    // {
    //     var customer = await customerRepository
    //         .FindByCondition(x => x.Id.Equals(id))
    //         .SingleOrDefaultAsync();
    //
    //     if (customer == null) return Results.NotFound();
    //
    //     await customerRepository.DeleteAsync(customer);
    //     Console.WriteLine("Before SaveChangesAsync");
    //     await customerRepository.SaveChangesAsync();
    //     Console.WriteLine("After SaveChangesAsync");
    //
    //     return Results.NoContent();
    // });

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

    app.UseAuthorization();

    app.MapControllers();

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