using Contracts.Domains.Core;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    // Controller call Service (different from Controller directly calling repo of Product.API)
    public async Task<IResult> GetCustomerByUsernameAsync(string username)
    {
        var customer = await _repository.GetCustomerByUsernameAsync(username);
        if (customer is null)
        {
            throw new CustomerNotFoundException(username);
        }

        return Results.Ok(customer);
    }

    // public async Task<IResult> GetCustomersAsync() => Results.Ok(await _repository.GetCustomersAsync());
}