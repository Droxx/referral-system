using ReferralService.Data;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;
using ReferralService.Data.Repositories.InMemory;

namespace ReferralService.DependencyInjection;

public static class ServiceCollection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Referral>, ReferralRepository>();
        
        return services;
    }
}