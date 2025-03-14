using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order, long, OrderContext>, IOrderRepository
{
    public OrderRepository(OrderContext dbContext, IUnitOfWork<OrderContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<Order?> GetOrder(long id) => await GetByIdAsync(id);

    public async Task<IEnumerable<Order>> GetOrdersByUsername(string username) =>
        await FindByCondition(o => o.Username != null && o.Username.Equals(username)).ToListAsync();

    public async Task CreateOrder(Order order) => await CreateAsync(order);

    public Task UpdateOrder(Order order) => UpdateAsync(order);

    public async Task DeleteOrder(long id)
    {
        var order = await GetOrder(id);
        if (order is not null) _ = DeleteAsync(order);
    }
}