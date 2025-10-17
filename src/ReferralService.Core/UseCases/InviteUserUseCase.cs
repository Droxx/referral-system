using Microsoft.Extensions.Logging;
using ReferralService.Core.Services;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;

namespace ReferralService.Core.UseCases;

public record InviteUserUseCaseInput(Guid UserId, string Email);

public interface IInviteUserUseCase : IUseCase<InviteUserUseCaseInput, Referral?>;

public class InviteUserUseCase(
    ILogger<InviteUserUseCase> logger,
    IEmailService emailService,
    IRepository<Referral> repository) : IInviteUserUseCase
{
    public async Task<Referral?> Handle(InviteUserUseCaseInput input, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Inviting email: {input.Email} from user: {input.UserId}");
        
        // TODO: Here I would add a check to see if the InviteeID exists in the system.
        // TODO: I would also check in the user-system to see if the invited email is already registered.
        
        var acceptedReferrals = await repository.Search(r =>
            r.InvitedEmail == input.Email &&
            (r.Status != ReferralStatus.Pending), cancellationToken);

        if (acceptedReferrals.Any())
        {
            logger.LogInformation("Email: {Email} has already been referred and accepted.", input.Email);
            return null;
        }
        
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            InvitedById = input.UserId,
            InvitedEmail = input.Email,
            InvitedAtUtc = DateTime.UtcNow,
            Status = ReferralStatus.Pending
        };

        await emailService.SendInviteEmail(referral.InvitedEmail);
        
        await repository.Store(referral, cancellationToken);
        await repository.SaveChanges(cancellationToken);
        return referral;
    }
}