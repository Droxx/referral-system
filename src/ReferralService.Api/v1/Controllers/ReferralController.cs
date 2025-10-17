using Microsoft.AspNetCore.Mvc;
using ReferralService.Contracts.v1;
using ReferralService.Core.UseCases;
using ReferralService.v1.Mappers;

namespace ReferralService.v1.Controllers;

[ApiController]
[Route("v1/referrals")]
public class ReferralController(ILogger<ReferralController> logger) : ControllerBase
{
    /// <summary>
    /// Endpoint to handle invitation sent webhooks.
    /// </summary>
    /// <param name="payload"><inheritdoc cref="Invitation"/></param>
    /// <returns>Guid of the newly created referral entity</returns>
    [HttpPost]
    [Route("invite-user")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> InvitationSent([FromBody] Invitation payload, [FromServices] InviteUserUseCase useCase)
    {
        var referral = await useCase.Handle(new InviteUserUseCaseInput(payload.InvitedById, payload.InvitedEmail));
        return Ok(referral.Id);
    }
    
    /// <summary>
    /// Gets a single referral by its ID.
    /// </summary>
    /// <param name="referralId">ID of referral entity</param>
    /// <returns>200OK and contract for the referral</returns>
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