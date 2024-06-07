using System;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
{
    public VillaNumberRepository(ApplicationDbContext db) : base(db)
    {
    }
    public override async Task CreateAsync(VillaNumber entity)
    {
        entity.CreatedDate = DateTime.Now;
        await base.CreateAsync(entity);
    }
    public override async Task UpdateAsync(VillaNumber entity)
    {
        entity.UpdatedDate = DateTime.Now;
        await base.UpdateAsync(entity);
    }
}