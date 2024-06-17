using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _db;
    protected readonly DbSet<T> _dbSet;
    public Repository(ApplicationDbContext db) 
    {
        _db = db;
        _dbSet = db.Set<T>();
    }
    public virtual async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveAsync();
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracked = true
        , string? includeProperties = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (!tracked) {
            query = query.AsNoTracking();
        }

        query = query.Where(predicate);
        
        if (includeProperties is not null) {
            string[] includePropertiesArray = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var includeProperty in includePropertiesArray) {
                query = query.Include(includeProperty);
            }
        }

        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, bool tracked = true
        , string? includeProperties = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (!tracked) {
            query = query.AsNoTracking();
        }

        if (predicate is not null) {
            query = query.Where(predicate);
        }

        if (includeProperties is not null) {
            string[] includePropertiesArray = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var includeProperty in includePropertiesArray) {
                query = query.Include(includeProperty);
            }
        }
        return await query.ToListAsync();
    }

    public virtual async Task RemoveAsync(T entity)
    {
        _db.Remove(entity);
        await SaveAsync();
    }

    public virtual async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveAsync();
    }
}