using Microsoft.EntityFrameworkCore;
using ReferralService.Core.UseCases;
using ReferralService.Data;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;
using ReferralService.Data.Repositories.InMemory;
using ReferralService.v1.Mappers;

namespace ReferralService.DependencyInjection;

public static class ServiceCollection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddDataServices();
        services.AddMappers();

        services.AddScoped<IGetReferralByIdUseCase, GetReferralByIdUseCase>();
        services.AddScoped<IUserInvitedUseCase, UserInvitedUseCase>();
        
        return services;
    }
    
    private static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddDbContext<ReferralServiceDbContext>(options =>
        {
            options.UseInMemoryDatabase("ReferralServiceDb");
        });
        services.AddScoped<IRepository<Referral>, ReferralRepository>();
        
        return services;
    }
    
    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddScoped<ReferralMapper>();
        
        return services;
    }
}