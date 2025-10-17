
using ReferralService.Data.Models;

namespace ReferralService.Data.Repositories;

public interface IRepository<T> where T : IRepositoryObject
{
    Task Store(T obj);
    Task Update(string reference, T obj);
    Task<T> Get(string reference);
    Task Delete(string reference);
    Task<bool> Exists(string reference);
}