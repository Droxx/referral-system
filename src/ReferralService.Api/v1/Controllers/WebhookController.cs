using Microsoft.AspNetCore.Mvc;
using ReferralService.Contracts.v1;
using ReferralService.Core.UseCases;

namespace ReferralService.v1.Controllers;

[ApiController]
[Route("v1/webhooks")]
public class WebhookController(ILogger<WebhookController> logger) : ControllerBase
{
    /// <summary>
    /// Webhook endpoint to handle user registered events.
    /// </summary>
    /// <param name="payload"><inheritdoc cref="UserRegisteredHookPayload"/></param>
    /// <returns>200OK when data processed</returns>
    [HttpPatch]
    [Route("user-registered")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UserRegistered([FromBody] UserRegisteredHookPayload payload, [FromServices] IUserRegisteredUseCase useCase)
    {
        await useCase.Handle(new UserRegisteredUseCaseInput(payload.UserId, payload.Email));
        return Ok();
    }
    
}