using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ReferralService.Data.Models;

namespace ReferralService.Data.Repositories.InMemory;

public class ReferralRepository(ReferralServiceDbContext context, ILogger<ReferralRepository> logger) : BaseMemoryRepository<Referral>(context, logger)
{
    protected override string SetKey => "Referrals";
    protected override DbSet<Referral> Set => context.Referrals;
}