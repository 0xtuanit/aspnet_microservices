namespace Customer.API.Services.Interfaces;

public interface ICustomerService
{
    Task<IResult> GetCustomerByUsernameAsync(string username);
    Task<IResult> GetCustomers();
    Task<long> CreateCustomer(Entities.Customer customer);
}