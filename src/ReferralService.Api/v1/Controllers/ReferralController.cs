using Microsoft.AspNetCore.Mvc;
using ReferralService.Contracts.v1;
using ReferralService.Core.UseCases;
using ReferralService.v1.Mappers;

namespace ReferralService.v1.Controllers;

[ApiController]
[Route("v1/referrals")]
public class ReferralController(ILogger<ReferralController> logger) : ControllerBase
{
    [HttpGet]
    [Route("{referralId:guid}")]
    [ProducesResponseType(typeof(Referral), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReferralById(
        [FromRoute] Guid referralId, 
        [FromServices] IGetReferralByIdUseCase useCase,
        [FromServices] ReferralMapper mapper)
    {
        var referral = await useCase.Handle(referralId);
        return Ok(mapper.ToContract(referral));
    }
}