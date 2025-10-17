namespace ReferralService.Core.Services;

public interface ICreditService
{
    Task MutateCredits(Guid userId, int amount, string reason);
}