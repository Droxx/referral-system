using Microsoft.Extensions.Logging;
using ReferralService.Core.Services;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;

namespace ReferralService.Core.UseCases;

public record UserRegisteredUseCaseInput(Guid UserId, string Email);

public interface IUserRegisteredUseCase : IUseCase<UserRegisteredUseCaseInput>;

public class UserRegisteredUseCase(
    ILogger<UserRegisteredUseCase> logger,
    ICreditService creditService,
    IRepository<Referral> repository) : IUserRegisteredUseCase
{
    public async Task Handle(UserRegisteredUseCaseInput input, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Registered user: {input.UserId} with email: {input.Email}. Checking for referrals.");
        // Get all referrals for this email
        var referrals = await repository.Search(r => r.InvitedEmail == input.Email && r.Status == ReferralStatus.Pending, cancellationToken);
        
        if (referrals.Any())
        {
            // Grab most recent referral and mark as accepted
            var mostRecentReferral = referrals
                .OrderByDescending(r => r.InvitedAtUtc)
                .First();
            mostRecentReferral.ReferredUserId = input.UserId; 
            mostRecentReferral.Status = ReferralStatus.Accepted;
            await repository.Update(mostRecentReferral.Id, mostRecentReferral, cancellationToken);
            
            await creditService.MutateCredits(mostRecentReferral.InvitedById, 10, 
                $"Referral bonus for inviting: {mostRecentReferral.InvitedEmail}");
            
            // Mark all other referrals as expired
            foreach (var expiredReferral in referrals.Where(r => r.Id != mostRecentReferral.Id))
            {
                expiredReferral.Status = ReferralStatus.Expired;
                await repository.Update(expiredReferral.Id, expiredReferral, cancellationToken);
            }
            
            await repository.SaveChanges(cancellationToken);
        }
    }
}