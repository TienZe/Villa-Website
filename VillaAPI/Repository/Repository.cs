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
    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracked = true)
    {
        var query = _dbSet.AsQueryable();
        
        if (!tracked) {
            query = query.AsNoTracking();
        }

        query = query.Where(predicate);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (predicate is not null) {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        _db.Remove(entity);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveAsync();
    }
}