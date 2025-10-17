using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ReferralService.Data.Models;

namespace ReferralService.Data.Repositories.InMemory;

public class ReferralRepository(DbContext context, ILogger<ReferralRepository> logger) : BaseMemoryRepository<Referral>(context, logger)
{
    protected override string SetKey => "Referrals";
}