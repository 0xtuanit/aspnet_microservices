using Contracts.Domains.Interfaces;
using Infrastructure.Common.Repositories;
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

    public async Task<IEnumerable<Order?>> GetOrdersByUsernameAsync(string username) =>
        await FindByCondition(o => o.Username != null && o.Username.Equals(username))
            .ToListAsync();

    public void CreateOrder(Order order) => Create(order);

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        await UpdateAsync(order);
        return order;
    }

    public async Task<Order?> GetOrder(long id) => await GetByIdAsync(id);

    public void DeleteOrder(Order order) => Delete(order);
}