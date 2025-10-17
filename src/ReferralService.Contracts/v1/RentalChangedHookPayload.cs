namespace ReferralService.Contracts.v1;

public class RentalChangedHookPayload
{
    public Guid RentalId { get; set; }
    public Guid RenterId { get; set; }
    public Guid OwnerId { get; set; }
    public RentalState State { get; set; }
}