using System;
using System.Linq.Expressions;

namespace VillaAPI.Repository.IRepository;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, bool tracked = true
        , string? includeProperties = null);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracked = true
        , string? includeProperties = null);

    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
    Task SaveAsync();
}