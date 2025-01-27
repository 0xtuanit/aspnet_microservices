using Customer.API.Services.Interfaces;

namespace Customer.API.Controllers;

public static class CustomersController
{
    public static void MapCustomersApi(this WebApplication app)
    {
        // Map all APIs into this controller
        app.MapGet("/api/customers/{username}", async (string username, ICustomerService customerService) =>
        {
            var result = await customerService.GetCustomerByUsernameAsync(username);
            return result ?? Results.NotFound();
        });
    }
}