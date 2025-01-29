namespace Contracts.Domains.Core;

public class CustomerNotFoundException(string username)
    : Exception($"The customer with the username = {username} was not found");