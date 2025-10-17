using Microsoft.EntityFrameworkCore;
using ReferralService.Core.Services;
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

        services.AddHttpClient<IEmailService, EmailService>();
        services.AddHttpClient<ICreditService, CreditService>();

        services.AddScoped<IGetReferralByIdUseCase, GetReferralByIdUseCase>();
        services.AddScoped<InviteUserUseCase, InviteUserUseCase>();
        services.AddScoped<IUserRegisteredUseCase, UserRegisteredUseCase>();
        services.AddScoped<IRentalChangedUseCase, RentalChangedUseCase>();
        services.AddScoped<IGetReferralsUseCase, GetReferralsUseCase>();
        
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