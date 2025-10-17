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
    /// <returns>Guid of the newly created referral entity. Or 208 if user is already referred</returns>
    [HttpPost]
    [Route("invite-user")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status208AlreadyReported)]
    public async Task<IActionResult> InvitationSent([FromBody] Invitation payload, [FromServices] IInviteUserUseCase useCase)
    {
        var referral = await useCase.Handle(new InviteUserUseCaseInput(payload.InvitedById, payload.InvitedEmail));
        if(referral == null)
        {
            logger.LogInformation("No referral created for invited email: {Email} as they have already been referred.", payload.InvitedEmail);
            return StatusCode(StatusCodes.Status208AlreadyReported);
        }
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

    /// <summary>
    /// Gets all referrals.
    /// </summary>
    /// <remarks>
    /// This endpoint I have added for testing purposes only.
    /// </remarks>
    /// <returns>200OK and list of all referalls in the system</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<Referral>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReferrals(
        [FromServices] IGetReferralsUseCase useCase,
        [FromServices] ReferralMapper mapper)
    {
        var referrals = await useCase.Handle(t => true);
        return Ok(referrals.Select(mapper.ToContract).ToList());
    }
}