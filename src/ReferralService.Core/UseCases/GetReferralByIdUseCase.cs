using Microsoft.Extensions.Logging;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;

namespace ReferralService.Core.UseCases;

public interface IGetReferralByIdUseCase : IUseCase<Guid, Referral>;

public class GetReferralByIdUseCase(
    ILogger<GetReferralByIdUseCase> logger,
    IRepository<Referral> repository) : IGetReferralByIdUseCase
{
    public async Task<Referral> Handle(Guid input, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Handling GetReferralByIdUseCase for ReferralId: {ReferralId}", input);
        return await repository.Get(input, cancellationToken);
    }
}