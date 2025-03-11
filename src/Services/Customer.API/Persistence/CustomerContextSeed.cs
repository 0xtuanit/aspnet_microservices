using Customer.API.Entities;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Customer.API.Persistence;

public static class CustomerContextSeed
{
    public static IHost SeedCustomerData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var customerContext = scope.ServiceProvider
            .GetRequiredService<CustomerContext>();
        customerContext.Database.MigrateAsync().GetAwaiter().GetResult();

        CreateCustomer(customerContext, "customer1", "first_name1", "last_name1", "customer1@gmail.com").GetAwaiter()
            .GetResult();
        CreateCustomer(customerContext, "customer2", "first_name2", "last_name2", "customer2@gmail.com").GetAwaiter()
            .GetResult();

        return host;
    }

    private static async Task CreateCustomer(CustomerContext customerContext, string username, string firstName,
        string lastName, string email)
    {
        var customer = await customerContext.Customers.SingleOrDefaultAsync(x => x.Username.Equals(username)
            || x.EmailAddress.Equals(email));
        if (customer == null)
        {
            var newCustomer = new Entities.Customer
            {
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email
            };
            await customerContext.Customers.AddAsync(newCustomer);
            await customerContext.SaveChangesAsync();
        }
    }
}