using System.Linq.Expressions;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Contracts.Common.Interfaces;

// For Query
public interface IRepositoryQueryBase<T, TK> where T : EntityBase<TK>
{
    IQueryable<T> FindAll(bool trackChanges = false);

    IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties);

    Task<T?> GetByIdAsync(TK id);

    Task<T?> GetByIdAsync(TK id, params Expression<Func<T, object>>[] includeProperties);
}

// For Command
public interface IRepositoryBaseAsync<T, TK> : IRepositoryQueryBase<T, TK>
    where T : EntityBase<TK>
{
    Task<TK> CreateAsync(T entity);

    Task<IList<TK>> CreateListAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);

    Task UpdateListAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);

    Task DeleteListAsync(IEnumerable<T> entities);

    Task<int> SaveChangesAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();

    Task EndTransactionAsync();

    Task RollbackTransactionAsync();
}

//-- Isolated these 2 ones with TContext so that we can work with MongoDB
public interface IRepositoryQueryBase<T, TK, TContext> : IRepositoryQueryBase<T, TK>
    where T : EntityBase<TK>
    where TContext : DbContext
{
}

public interface IRepositoryBaseAsync<T, TK, TContext> : IRepositoryBaseAsync<T, TK>
    where T : EntityBase<TK>
    where TContext : DbContext
{
}