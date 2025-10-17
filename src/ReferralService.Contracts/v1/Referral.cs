namespace ReferralService.Contracts.v1;

public class Referral
{
    /// <summary>
    /// ID of referral record
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// ID of the user who made the referral
    /// </summary>
    public Guid InvitedById { get; set; }
    
    /// <summary>
    /// Email of the person being invited
    /// </summary>
    public required string InvitedEmail { get; set; }
    
    /// <summary>
    /// ID of the referred user, once they have registered
    /// </summary>
    public Guid? ReferredUserId { get; set; }
    
    /// <summary>
    /// Date and time when the invitation was sent (in UTC)
    /// </summary>
    public DateTime InvitedAtUtc { get; set; }
    
    /// <summary>
    /// Current status of the referral
    /// </summary>
    public ReferralStatus Status { get; set; }
}