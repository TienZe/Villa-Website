using System.Linq.Expressions;

namespace VillaAPI.Repository.IRepository;

public interface IVillaRepository
{
    Task<List<Villa>> GetAll(Expression<Func<Villa, bool>>? predicate = null);
    Task<Villa> Get(Expression<Func<Villa, bool>>? predicate = null, bool tracked = true);
    Task Create(Villa villa);
    Task Remove(Villa villa);
    Task Save();
}