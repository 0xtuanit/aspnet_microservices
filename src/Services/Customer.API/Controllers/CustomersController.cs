using Contracts.Domains.Core;
using Customer.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Customer;
using AutoMapper;

namespace Customer.API.Controllers;

public static class CustomersController
{
    public static void MapCustomersApi(this WebApplication app)
    {
        // Map all APIs into this controller
        app.MapGet("/api/customers", (ICustomerService customerService) =>
            customerService.GetCustomers());

        app.MapGet("/api/customers/{username}", async (string username, ICustomerService customerService) =>
        {
            try
            {
                var customer = await customerService.GetCustomerByUsernameAsync(username);
                return customer;
            }
            catch (CustomerNotFoundException e)
            {
                return Results.NotFound(e.Message);
            }
        });

        app.MapPost("/api/customers",
            async ([FromBody] CreateOrUpdateCustomerDto customerDto, ICustomerService customerService,
                IMapper mapper) =>
            {
                var customer = mapper.Map<Entities.Customer>(customerDto);
                await customerService.CreateCustomer(customer);
            });

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
    }
}