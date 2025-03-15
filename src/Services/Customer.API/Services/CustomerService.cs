using AutoMapper;
using Contracts.Domains.Core;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // Controller call Service (different from Controller directly calling repo of Product.API)
    public async Task<IResult> GetCustomerByUsernameAsync(string username)
    {
        var entity = await _repository.GetCustomerByUsernameAsync(username);

        if (entity is null) throw new CustomerNotFoundException(username);

        var result = _mapper.Map<CustomerDto>(entity);

        return Results.Ok(result);
    }

    public async Task<IResult> GetCustomers() => Results.Ok(await _repository.GetCustomersAsync());
}