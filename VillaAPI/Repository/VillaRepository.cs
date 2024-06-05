using VillaAPI.Data;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class VillaRepository : Repository<Villa>, IVillaRepository
{
    public VillaRepository(ApplicationDbContext db) : base(db)
    {
    }
}