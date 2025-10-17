using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ReferralService.Core.Services;
using ReferralService.Core.UseCases;
using ReferralService.Data.Models;

namespace RentalService.UnitTests;

public class RentalChangedFlow : TestBase
{
    private readonly Mock<ICreditService> _creditService = new();

    private readonly IRentalChangedUseCase _sut;
    
    public RentalChangedFlow()
    {
        _sut = new RentalChangedUseCase(
            NullLogger<RentalChangedUseCase>.Instance,
            ReferralRepository,
            _creditService.Object);
    }
    
    [Fact]
    public async Task ReferralExistsInAcceptedState_RentalFinished_UpdatesReferralAndCreditsReferrer()
    {
        var referral = new Referral()
        {
            Id = Guid.NewGuid(),
            InvitedById = Guid.NewGuid(),
            ReferredUserId = Guid.NewGuid(),
            Status = ReferralStatus.Accepted,
            InvitedEmail = "test@email.com",
            InvitedAtUtc = DateTime.UtcNow.AddDays(-10)
        };
        await ReferralRepository.Store(referral);
        await ReferralRepository.SaveChanges();
        
        var input = new RentalChangedUseCaseInput(
            Guid.NewGuid(),
            referral.ReferredUserId.Value,
            Guid.NewGuid(),
            RentalState.Finished);
        
        await _sut.Handle(input);
        
        var dbRef = await ReferralRepository.Get(referral.Id);
        Assert.Equal(ReferralStatus.Completed, dbRef.Status);
        
        _creditService.Verify(s => s.MutateCredits(referral.ReferredUserId.Value, 5, 
            It.Is<string>(msg => msg.Contains(input.RentalId.ToString()))));
    }

    [Theory]
    [InlineData(ReferralStatus.Completed)]
    [InlineData(ReferralStatus.Expired)]
    [InlineData(ReferralStatus.Pending)]
    public async Task ReferralExistsInInvalidState_RentalFinished_NoActionTaken(ReferralStatus status)
    {
        var referral = new Referral()
        {
            Id = Guid.NewGuid(),
            InvitedById = Guid.NewGuid(),
            ReferredUserId = Guid.NewGuid(),
            Status = status,
            InvitedEmail = "test@email.com",
            InvitedAtUtc = DateTime.UtcNow.AddDays(-10)
        };
        await ReferralRepository.Store(referral);
        await ReferralRepository.SaveChanges();

        var input = new RentalChangedUseCaseInput(
            Guid.NewGuid(),
            referral.ReferredUserId.Value,
            Guid.NewGuid(),
            RentalState.Finished);

        await _sut.Handle(input);

        var dbRef = await ReferralRepository.Get(referral.Id);
        Assert.Equal(status, dbRef.Status);
        _creditService.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(ReferralStatus.Pending)]
    [InlineData(ReferralStatus.Accepted)]
    [InlineData(ReferralStatus.Expired)]
    [InlineData(ReferralStatus.Completed)]
    public async Task RentalInProgressOnAllReferralStatus_NoActionTaken(ReferralStatus status)
    {
        var referral = new Referral()
        {
            Id = Guid.NewGuid(),
            InvitedById = Guid.NewGuid(),
            ReferredUserId = Guid.NewGuid(),
            Status = status,
            InvitedEmail = "test@email.com",
            InvitedAtUtc = DateTime.UtcNow.AddDays(-10)
        };
        await ReferralRepository.Store(referral);
        await ReferralRepository.SaveChanges();

        var input = new RentalChangedUseCaseInput(
            Guid.NewGuid(),
            referral.ReferredUserId.Value,
            Guid.NewGuid(),
            RentalState.InProgress);

        await _sut.Handle(input);

        var dbRef = await ReferralRepository.Get(referral.Id);
        Assert.Equal(status, dbRef.Status);
        _creditService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task NoReferralExists_RentalFinished_NoActionTaken()
    {
        var input = new RentalChangedUseCaseInput(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            RentalState.Finished);

        await _sut.Handle(input);
        _creditService.VerifyNoOtherCalls();
    }
}