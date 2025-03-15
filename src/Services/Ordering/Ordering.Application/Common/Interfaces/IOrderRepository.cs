using Contracts.Common.Interfaces;
using Ordering.Domain.Entities;

namespace Ordering.Application.Common.Interfaces;

public interface IOrderRepository : IRepositoryBaseAsync<Order, long>
{
    Task<IEnumerable<Order>> GetOrdersByUsernameAsync(string username);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderAsync(Order order);
}