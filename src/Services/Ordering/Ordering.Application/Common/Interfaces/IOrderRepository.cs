using Contracts.Common.Interfaces;
using Ordering.Domain.Entities;

namespace Ordering.Application.Common.Interfaces;

public interface IOrderRepository : IRepositoryBaseAsync<Order, long>
{
    Task<Order?> GetOrder(long id);
    Task<IEnumerable<Order>> GetOrdersByUsername(string username);
    Task CreateOrder(Order order);
    Task<Order> CreateOrder2(Order order);
    Task UpdateOrder(Order order);
    Task DeleteOrder(long id);
}