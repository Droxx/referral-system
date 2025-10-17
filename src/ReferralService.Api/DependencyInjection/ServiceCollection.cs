using Microsoft.EntityFrameworkCore;
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
        
        return services;
    }
    
    private static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddDbContext<DbContext>(options =>
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