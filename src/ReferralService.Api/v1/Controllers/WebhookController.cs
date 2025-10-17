using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReferralService.Contracts.v1;
using ReferralService.Core.UseCases;

namespace ReferralService.v1.Controllers;

[ApiController]
[Route("v1/webhooks")]
public class WebhookController(ILogger<WebhookController> logger) : ControllerBase
{
    /// <summary>
    /// Endpoint to handle invitation sent webhooks.
    /// </summary>
    /// <param name="payload"><inheritdoc cref="InvitationHookPayload"/></param>
    /// <returns>Guid of the newly created referral entity</returns>
    [HttpPost]
    [Route("invitation-sent")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> InvitationSent([FromBody] InvitationHookPayload payload, [FromServices] IUserInvitedUseCase useCase)
    {
        var referral = await useCase.Handle(new UserInvitedUseCaseInput(payload.InvitedById, payload.InvitedEmail));
        return Ok(referral.Id);
    }
}