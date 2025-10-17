using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ReferralService.Data.Models;

namespace ReferralService.Data.Repositories.InMemory;

/// <summary>
/// Base implementation of an in-memory repository using IMemoryCache.
/// </summary>
public abstract class BaseMemoryRepository<T>(ReferralServiceDbContext context, ILogger<BaseMemoryRepository<T>> logger) : IRepository<T> where T : class, IRepositoryObject
{
    protected abstract string SetKey { get; }
    protected abstract DbSet<T> Set { get; }
    
    public async Task Store(T obj, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Storing {typeof(T).Name} with Id: {obj.Id}");
        if(await Set.AnyAsync(e => e.Id == obj.Id, cancellationToken))
            throw new DuplicateNameException($"An object with Id {obj.Id} already exists.");
        await Set.AddAsync(obj, cancellationToken);
    }

    public async Task Update(Guid id, T obj, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Updating {typeof(T).Name} with Id: {obj.Id}");
        var existing = await Set.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (existing == null)
            throw new KeyNotFoundException($"No object found with Id {id}.");

        context.Entry(existing).CurrentValues.SetValues(obj);
    }

    public Task<List<T>> Search(Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Searching {typeof(T).Name} with given predicate");
        return Task.FromResult(Set
            .Where(predicate)
            .ToList());
    }

    public async Task<T> Get(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Getting {typeof(T).Name} with Id: {id}");
        var obj = await Set.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (obj == null)
            throw new KeyNotFoundException($"No object found with Id {id}.");
        return obj;
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    { 
        var entity = await Get(id, cancellationToken);
        await Delete(entity, cancellationToken);
    }

    public Task Delete(T obj, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Deleting {typeof(T).Name} with Id: {obj.Id}");
        Set.Remove(obj);
        return Task.CompletedTask; 
    }

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Checking existence of {typeof(T).Name} with Id: {id}");
        return await Set.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}