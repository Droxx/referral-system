using System.ComponentModel.DataAnnotations;

namespace ReferralService.Contracts.v1;

/// <summary>
/// Model representing the payload for an invitation hook.
/// </summary>
public class Invitation
{
    /// <summary>
    /// ID of the user who sent the invitation.
    /// </summary>
    [Required]
    public required Guid InvitedById { get; set; }
    
    /// <summary>
    /// Email of the person being invited.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string InvitedEmail { get; set; }
}