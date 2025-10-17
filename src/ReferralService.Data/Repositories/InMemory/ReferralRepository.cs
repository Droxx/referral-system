using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ReferralService.Data.Models;

namespace ReferralService.Data.Repositories.InMemory;

public class ReferralRepository(IMemoryCache cache, ILogger<ReferralRepository> logger) : BaseMemoryRepository<Referral>(cache, logger)
{
    protected override string CacheKey => "Referral";
}