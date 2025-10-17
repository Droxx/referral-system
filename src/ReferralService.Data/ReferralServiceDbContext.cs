using Microsoft.EntityFrameworkCore;
using ReferralService.Data.Models;

namespace ReferralService.Data;

public class ReferralServiceDbContext : DbContext
{
    public ReferralServiceDbContext(DbContextOptions<ReferralServiceDbContext> options) : base(options)
    {
    }
    
    public DbSet<Referral> Referrals { get; set; }
}