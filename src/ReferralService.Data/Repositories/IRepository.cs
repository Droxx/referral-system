
using ReferralService.Data.Models;

namespace ReferralService.Data.Repositories;

public interface IRepository<T> where T : IRepositoryObject
{
    Task Store(T obj, CancellationToken cancellationToken = default);
    Task Update(Guid id, T obj, CancellationToken cancellationToken = default);
    Task<List<T>> Search(Func<T, bool> predicate, CancellationToken cancellationToken = default);
    Task<T> Get(Guid id, CancellationToken cancellationToken = default);
    Task Delete(Guid id, CancellationToken cancellationToken = default);
    Task Delete(T obj, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
    Task SaveChanges(CancellationToken cancellationToken = default);
}