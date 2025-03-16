using Customer.API.Persistence;
using Customer.API.Repositories.Interfaces;
using Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories;

public class CustomerRepository : RepositoryQueryBase<Entities.Customer, int, CustomerContext>, ICustomerRepository
{
    public CustomerRepository(CustomerContext dbContext) : base(dbContext)
    {
    }

    public Task<Entities.Customer?> GetCustomerByUsernameAsync(string username) =>
        FindByCondition(x => x.Username != null && x.Username.Equals(username)).SingleOrDefaultAsync();

    public async Task<IEnumerable<Entities.Customer>> GetCustomersAsync() => await FindAll().ToListAsync();
}