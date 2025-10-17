using Microsoft.Extensions.Logging;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;

namespace ReferralService.Core.UseCases;

public record UserInvitedUseCaseInput(Guid UserId, string Email);

public interface IUserInvitedUseCase : IUseCase<UserInvitedUseCaseInput, Referral>;

public class UserInvitedUseCase(
    ILogger<UserInvitedUseCase> logger,
    IRepository<Referral> repository) : IUserInvitedUseCase
{
    public async Task<Referral> Handle(UserInvitedUseCaseInput input, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Invited email: {input.Email} by user: {input.UserId}");
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            InvitedById = input.UserId,
            InvitedEmail = input.Email,
            InvitedAtUtc = DateTime.UtcNow,
            Status =ReferralStatus.Pending
        };
        await repository.Store(referral);
        await repository.SaveChanges();
        return referral;
    }
}