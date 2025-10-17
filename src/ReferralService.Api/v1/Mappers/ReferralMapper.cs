using ReferralService.Helpers;
using DataReferral = ReferralService.Data.Models.Referral;
using ContractReferral = ReferralService.Contracts.v1.Referral;

namespace ReferralService.v1.Mappers;

public class ReferralMapper : IMapper<DataReferral, ContractReferral>
{
    public ContractReferral ToContract(DataReferral input)
    {
        return new()
        {
            Id  = input.Id,
            Status = Enum.Parse<Contracts.v1.ReferralStatus>(input.Status.ToString()),
            InvitedById = input.InvitedById,
            InvitedEmail = input.InvitedEmail,
            ReferredUserId = input.ReferredUserId,
            InvitedAtUtc = input.InvitedAtUtc,
            RentalId = input.RentalId
        };
    }

    public DataReferral ToData(ContractReferral input)
    {
        return new()
        {
            Id  = input.Id,
            Status = Enum.Parse<Data.Models.ReferralStatus>(input.Status.ToString()),
            InvitedById = input.InvitedById,
            InvitedEmail = input.InvitedEmail,
            ReferredUserId = input.ReferredUserId,
            InvitedAtUtc = input.InvitedAtUtc,
            RentalId = input.RentalId
        };
    }
}