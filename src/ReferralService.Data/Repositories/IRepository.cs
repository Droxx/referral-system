
using ReferralService.Data.Models;

namespace ReferralService.Data.Repositories;

public interface IRepository<T> where T : IRepositoryObject
{
    Task Store(T obj);
    Task Update(Guid id, T obj);
    Task<List<T>> Search(Func<T, bool> predicate);
    Task<T> Get(Guid id);
    Task Delete(Guid id);
    Task Delete(T obj);
    Task<bool> Exists(Guid id);
    Task SaveChanges();
}