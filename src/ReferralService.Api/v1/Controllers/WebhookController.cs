using Microsoft.AspNetCore.Mvc;
using ReferralService.Contracts.v1;
using ReferralService.Core.UseCases;

namespace ReferralService.v1.Controllers;

[ApiController]
[Route("v1/webhooks")]
public class WebhookController(ILogger<WebhookController> logger) : ControllerBase
{
    /// <summary>
    /// Handle user registered events.
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
    
    /// <summary>
    /// Handle rental changed events.
    /// </summary>
    /// <param name="payload"><inheritdoc cref="RentalChangedHookPayload"/></param>
    /// <returns>200OK when data processed</returns>
    [HttpPatch]
    [Route("rental-changed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RentalChanged([FromBody] RentalChangedHookPayload payload, [FromServices] IRentalChangedUseCase useCase)
    {
        await useCase.Handle(new RentalChangedUseCaseInput(payload.RentalId, payload.RenterId, payload.OwnerId,
            Enum.Parse<Data.Models.RentalState>(payload.State.ToString())));
        return Ok();
    }
}