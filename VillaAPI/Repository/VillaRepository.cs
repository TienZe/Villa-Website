﻿using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class VillaRepository : Repository<Villa>, IVillaRepository
{
    public VillaRepository(ApplicationDbContext db) : base(db)
    {
    }

    public override async Task UpdateAsync(Villa entity)
    {
        entity.UpdatedDate = DateTime.Now;
        _dbSet.Update(entity);
        await SaveAsync();
    }
}