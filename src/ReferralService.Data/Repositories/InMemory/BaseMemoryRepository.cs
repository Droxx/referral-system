using System.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ReferralService.Data.Models;

namespace ReferralService.Data.Repositories.InMemory;

public abstract class BaseMemoryRepository<T>(IMemoryCache cache, ILogger<BaseMemoryRepository<T>> logger) : IRepository<T> where T : IRepositoryObject
{
    protected abstract string CacheKey { get; }
    
    private string GetItemKey(T item) => GetItemKey(item.Reference);
    private string GetItemKey(string reference) => $"{CacheKey}_{reference}";
    private static MemoryCacheEntryOptions CacheOptions => new ()
    {
        Priority = CacheItemPriority.NeverRemove
    };
    
    public Task Store(T obj)
    {
        logger.LogInformation("Storing object with reference {Reference} in cache under key {CacheKey}", obj.Reference, CacheKey);
        if(cache.TryGetValue(GetItemKey(obj), out _))
            throw new DuplicateNameException($"An item with reference {obj.Reference} already exists in the repository.");
        cache.Set(GetItemKey(obj), obj, CacheOptions);
        return Task.CompletedTask;
    }

    public Task Update(string reference, T obj)
    {
        logger.LogInformation("Updating object with reference {Reference} in cache under key {CacheKey}", reference, CacheKey);
        if(!cache.TryGetValue(GetItemKey(obj), out _))
            throw new KeyNotFoundException($"No item with reference {reference} exists in the repository.");
        cache.Set(GetItemKey(obj), obj, CacheOptions);
        return Task.CompletedTask;
    }

    public Task<T> Get(string reference)
    {
        logger.LogInformation("Retrieving object with reference {Reference} from cache under key {CacheKey}", reference, CacheKey);
        if(!cache.TryGetValue(GetItemKey(reference), out T? obj) || obj == null)
            throw new KeyNotFoundException($"No item with reference {reference} exists in the repository.");
        return Task.FromResult(obj);
    }

    public Task Delete(string reference)
    {
        logger.LogInformation("Deleting object with reference {Reference} from cache under key {CacheKey}", reference, CacheKey);
        if(!cache.TryGetValue(GetItemKey(reference), out _))
            throw new KeyNotFoundException($"No item with reference {reference} exists in the repository.");
        cache.Remove(GetItemKey(reference));
        return Task.CompletedTask;
    }

    public Task<bool> Exists(string reference)
    {
        logger.LogInformation("Checking existence of object with reference {Reference} in cache under key {CacheKey}", reference, CacheKey);
        return Task.FromResult(cache.TryGetValue(GetItemKey(reference), out _));
    }
}