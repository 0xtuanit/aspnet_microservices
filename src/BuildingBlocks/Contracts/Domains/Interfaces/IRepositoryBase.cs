using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Contracts.Domains.Interfaces;

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

//-- Isolated this one with TContext so that we can work with MongoDB
public interface IRepositoryQueryBase<T, K, TContext> : IRepositoryQueryBase<T, K>
    where T : EntityBase<K>
    where TContext : DbContext
{
}

// For Command
public interface IRepositoryBase<T, K> : IRepositoryQueryBase<T, K>
    where T : EntityBase<K>
{
    void Create(T entity);
    Task<K> CreateAsync(T entity);
    IList<K> CreateList(IEnumerable<T> entities);
    Task<IList<K>> CreateListAsync(IEnumerable<T> entities);
    void Update(T entity);
    Task UpdateAsync(T entity);
    void UpdateList(IEnumerable<T> entities);
    Task UpdateListAsync(IEnumerable<T> entities);
    void Delete(T entity);
    Task DeleteAsync(T entity);
    void DeleteList(IEnumerable<T> entities);
    Task DeleteListAsync(IEnumerable<T> entities);
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task EndTransactionAsync();
    Task RollbackTransactionAsync();
}

public interface IRepositoryBase<T, K, TContext> : IRepositoryBase<T, K>
    where T : EntityBase<K>
    where TContext : DbContext
{
}