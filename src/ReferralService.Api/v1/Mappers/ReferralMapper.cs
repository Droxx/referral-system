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
            Reference = input.Reference
        };
    }

    public DataReferral ToData(ContractReferral input)
    {
        return new()
        {
            Reference = input.Reference
        };
    }
}