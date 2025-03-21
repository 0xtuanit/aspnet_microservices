using Contracts.Domains.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interfaces;
using Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories;

public class CustomerRepository : RepositoryBase<Entities.Customer, long, CustomerContext>, ICustomerRepository
{
    public CustomerRepository(CustomerContext dbContext, IUnitOfWork<CustomerContext> unitOfWork) : base(dbContext,
        unitOfWork)
    {
    }

    public Task<Entities.Customer?> GetCustomerByUsernameAsync(string username) =>
        FindByCondition(x => x.Username != null && x.Username.Equals(username)).SingleOrDefaultAsync();

    public async Task<IEnumerable<Entities.Customer>> GetCustomersAsync() => await FindAll().ToListAsync();

    public Task<long> CreateCustomer(Entities.Customer customer) => CreateAsync(customer);
}