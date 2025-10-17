using Microsoft.Extensions.Logging;
using ReferralService.Core.Services;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;

namespace ReferralService.Core.UseCases;

public record RentalChangedUseCaseInput(Guid RentalId, Guid RenterId, Guid OwnerId, RentalState State);

public interface IRentalChangedUseCase : IUseCase<RentalChangedUseCaseInput>;

public class RentalChangedUseCase(
    ILogger<RentalChangedUseCase> logger,
    IRepository<Referral> repository,
    ICreditService creditService) : IRentalChangedUseCase
{
    public async Task Handle(RentalChangedUseCaseInput useCaseInput, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling rental changed for RentalId: {RentalId},State: {State}",
            useCaseInput.RentalId, useCaseInput.State);
        
        var referral = (await repository.Search(r =>
            r.ReferredUserId == useCaseInput.RenterId &&
                r.Status == ReferralStatus.Accepted, cancellationToken))
            .FirstOrDefault();

        if (referral == null)
        {
            logger.LogInformation("No accepted referral found for RenterId: {RenterId}", useCaseInput.RenterId);
            return;
        }

        if (useCaseInput.State == RentalState.Finished)
        {
            logger.LogInformation("Mutating credits for ReferrerId: {OwnerId} due to completed rental RentalId: {RentalId}",
                referral.InvitedById, useCaseInput.RentalId);
            
            await creditService.MutateCredits(useCaseInput.RenterId, 5, 
                $"Referral bonus for invited user {referral.ReferredUserId} completed rental: {useCaseInput.RentalId}");

            referral.Status = ReferralStatus.Completed;
            await repository.Update(referral.Id, referral, cancellationToken);
            await repository.SaveChanges(cancellationToken);
        }
    }
}