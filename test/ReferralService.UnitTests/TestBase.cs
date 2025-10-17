using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ReferralService.Data;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;
using ReferralService.Data.Repositories.InMemory;

namespace RentalService.UnitTests;

public class TestBase
{
    protected readonly IRepository<Referral> ReferralRepository;

    public TestBase()
    {
        var opBuilder =
            new DbContextOptionsBuilder<ReferralServiceDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        var dbContext = new ReferralServiceDbContext(opBuilder.Options);

        ReferralRepository = new ReferralRepository(dbContext, NullLogger<ReferralRepository>.Instance);
    }

    protected void ClearDb()
    {
        var allReferrals = ReferralRepository.Search(t => true).Result;
        foreach (var referral in allReferrals)
        {
            ReferralRepository.Delete(referral).Wait();
        }

        ReferralRepository.SaveChanges().Wait();
    }
    
    public void Dispose()
    {
        //ClearDb();
        
    }
}