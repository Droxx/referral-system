using Microsoft.Extensions.Logging;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;

namespace ReferralService.Core.UseCases;

public interface IGetReferralsUseCase : IUseCase<Func<Referral, bool>, List<Referral>>;

public class GetReferralsUseCase(
    ILogger<GetReferralsUseCase> logger,
    IRepository<Referral> repository) : IGetReferralsUseCase
{
    public async Task<List<Referral>> Handle(Func<Referral, bool> input, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all referrals");
        var result = await repository.Search(input, cancellationToken);
        return result;
    }
}