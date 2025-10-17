using System.ComponentModel.DataAnnotations;

namespace ReferralService.Contracts.v1;

/// <summary>
/// Model representing the payload for a user-registered hook.
/// </summary>
public class UserRegisteredHookPayload
{
    /// <summary>
    /// ID of the new user
    /// </summary>
    [Required]
    public required Guid UserId { get; set; }
    
    /// <summary>
    /// Email of the new user
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}