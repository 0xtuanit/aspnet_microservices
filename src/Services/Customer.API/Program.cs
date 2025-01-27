using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API.Controllers;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

// Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Start Customer API up");

try
{
    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(
        options => options.UseNpgsql(connectionString)
    );

    // We have to configure all these for declaring ICustomer repo & ICustomer service
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
        .AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBaseAsync<,,>))
        .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
        .AddScoped<ICustomerService, CustomerService>();

    var app = builder.Build();

    // Map URL following minimal API style
    app.MapGet("/", () => "Welcome to Customer API!");
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
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.SeedCustomerData().Run();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information("Shut down Customer API complete");
    Log.CloseAndFlush();
}