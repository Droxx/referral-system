using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ReferralService.Core.Services;
using ReferralService.Core.UseCases;
using ReferralService.Data.Models;
using ReferralService.Data.Repositories;

namespace RentalService.UnitTests;

public class UserRegisteredFlow : TestBase
{
    private readonly Mock<ICreditService> _creditService = new();
    private readonly IUserRegisteredUseCase _sut;
    
    public UserRegisteredFlow()
    {
        _sut = new UserRegisteredUseCase(
            NullLogger<UserRegisteredUseCase>.Instance,
            _creditService.Object,
            ReferralRepository);
    }
    
    [Fact]
    public async Task InviteExists_MovesStateAndCreditsReferrer()
    {
        var invite = new Referral()
        {
            InvitedById = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            Status = ReferralStatus.Pending,
            InvitedEmail = "test@email.com",
            InvitedAtUtc = DateTime.UtcNow,
        };
        await ReferralRepository.Store(invite);
        await ReferralRepository.SaveChanges();
        
        var input = new UserRegisteredUseCaseInput(Guid.NewGuid(), invite.InvitedEmail);
        await _sut.Handle(input);
        
        var dbRef = await ReferralRepository.Get(invite.Id);
        Assert.Equal(ReferralStatus.Accepted, dbRef.Status);
        Assert.Equal(input.UserId, dbRef.ReferredUserId);
        
        _creditService.Verify(s => s.MutateCredits(invite.InvitedById, 10, 
            It.Is<string>(msg => msg.Contains(invite.InvitedEmail))));
    }

    [Fact]
    public async Task InviteDoesNotExist_NoActionTaken()
    {
        var input = new UserRegisteredUseCaseInput(Guid.NewGuid(), "test@email.com");

        await _sut.Handle(input);
        var dbRefs = await ReferralRepository.Search(r => r.InvitedEmail == input.Email);
        Assert.Empty(dbRefs);
        
        _creditService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task MultipleInvitesExist_OnlyMostRecentAcceptedAndOlderMarkedExpired()
    {
        var email = "test@email.com";
        var olderInvite = new Referral()
        {
            InvitedById = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            Status = ReferralStatus.Pending,
            InvitedEmail = email,
            InvitedAtUtc = DateTime.UtcNow.AddDays(-1),
        };
        var recentInvite = new Referral()
        {
            InvitedById = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            Status = ReferralStatus.Pending,
            InvitedEmail = email,
            InvitedAtUtc = DateTime.UtcNow,
        };
        await ReferralRepository.Store(olderInvite);
        await ReferralRepository.Store(recentInvite);
        await ReferralRepository.SaveChanges();

        var input = new UserRegisteredUseCaseInput(Guid.NewGuid(), email);
        await _sut.Handle(input);

        var dbOlderRef = await ReferralRepository.Get(olderInvite.Id);
        Assert.Equal(ReferralStatus.Expired, dbOlderRef.Status);
        var dbRecentRef = await ReferralRepository.Get(recentInvite.Id);
        Assert.Equal(ReferralStatus.Accepted, dbRecentRef.Status);

        _creditService.Verify(s => s.MutateCredits(recentInvite.InvitedById, 10,
            It.Is<string>(msg => msg.Contains(email))), Times.Once);
    }

    [Theory]
    [InlineData(ReferralStatus.Accepted)]
    [InlineData(ReferralStatus.Completed)]
    [InlineData(ReferralStatus.Expired)]
    public async Task DoesNotProcessInvalidStatus(ReferralStatus status)
    {
        var invite = new Referral()
        {
            InvitedById = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            Status = status,
            InvitedEmail = "test@email.com",
            InvitedAtUtc = DateTime.UtcNow,
        };
        await ReferralRepository.Store(invite);
        await ReferralRepository.SaveChanges();

        var input = new UserRegisteredUseCaseInput(Guid.NewGuid(), invite.InvitedEmail);
        await _sut.Handle(input);
        var dbRef = await ReferralRepository.Get(invite.Id);
        Assert.Equal(status, dbRef.Status);
        _creditService.VerifyNoOtherCalls();
    }

}