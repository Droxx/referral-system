using ReferralService.Data.Models;

namespace ReferralService.Helpers;

public interface IMapper<TData, TContract> where TData : IRepositoryObject
{
    TContract ToContract(TData input);
    TData ToData(TContract input);
}