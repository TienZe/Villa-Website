using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class VillaRepository : IVillaRepository
{
    private readonly ApplicationDbContext _db;
    public VillaRepository(ApplicationDbContext db) 
    {
        _db = db;        
    }
    public async Task Create(Villa villa)
    {
        await _db.Villas.AddAsync(villa);
        await Save();
    }

    public async Task<Villa> Get(Expression<Func<Villa, bool>>? predicate = null, bool tracked = true)
    {
        var query = _db.Villas.AsQueryable();
        
        if (!tracked) {
            query = query.AsNoTracking();
        }

        if (predicate is not null) {
            return await query.FirstAsync(predicate);
        }

        return await query.FirstAsync();
    }

    public async Task<List<Villa>> GetAll(Expression<Func<Villa, bool>>? predicate = null)
    {
        var query = _db.Villas.AsQueryable();
        
        if (predicate is not null) {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }

    public async Task Remove(Villa villa)
    {
        _db.Remove(villa);
        await Save();
    }

    public async Task Save()
    {
        await _db.SaveChangesAsync();
    }
}