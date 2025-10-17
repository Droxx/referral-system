namespace ReferralService.Data.Models;

public class Referral : IRepositoryObject
{
    /// <summary>
    /// ID of referral record
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// ID of the user who made the referral
    /// </summary>
    public required Guid InvitedById { get; set; }
    
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
    public required DateTime InvitedAtUtc { get; set; }
    
    /// <summary>
    /// Current status of the referral
    /// </summary>
    public required ReferralStatus Status { get; set; }
    
    /// <summary>
    /// ID of the first rental made by the referred user, if any
    /// </summary>
    public Guid? RentalId { get; set; }
}

public enum ReferralStatus
{
    Pending,
    Expired,
    Accepted,
    Completed
}