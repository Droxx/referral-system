using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ReferralService.Core.Services;
using ReferralService.Core.UseCases;
using ReferralService.Data.Models;

namespace RentalService.UnitTests;

public class InviteUserFlow : TestBase, IDisposable
{
    private readonly Mock<IEmailService> _emailService = new();

    private IInviteUserUseCase _sut;
    
    public InviteUserFlow()
    {
        _sut = new InviteUserUseCase(NullLogger<InviteUserUseCase>.Instance, _emailService.Object, ReferralRepository);
    }
    
    [Fact]
    public async Task NoReferralExists_CreatesNewReferral()
    {
        var inviterId = Guid.NewGuid();
        var inviteEmail = "test@test.com";
        var input = new InviteUserUseCaseInput(inviterId, inviteEmail);
        
        var referral = await _sut.Handle(input);
        
        _emailService.Verify(s => s.SendInviteEmail(It.Is<string>(e => e == inviteEmail)));

        var dbRef = await ReferralRepository.Get(referral.Id);
        Assert.NotNull(dbRef);
        Assert.Equal(inviterId, dbRef.InvitedById);
    }

    [Fact]
    public async Task PendingReferralExistsWithSameEmail_CreatesNewReferral()
    {
        var dbRef = new Referral()
        {
            Id = Guid.NewGuid(),
            InvitedById = Guid.NewGuid(),
            InvitedEmail = "test@test.com",
            Status = ReferralStatus.Pending,
            InvitedAtUtc = new DateTime(1999, 1, 1)
        };
        await ReferralRepository.Store(dbRef);
        await ReferralRepository.SaveChanges();
        
        var referral = await _sut.Handle(new (dbRef.InvitedById, dbRef.InvitedEmail));
        
        _emailService.Verify(s => s.SendInviteEmail(It.Is<string>(e => e == dbRef.InvitedEmail)));
        
        var dbRefs = await ReferralRepository.Search(r => r.InvitedEmail == dbRef.InvitedEmail);
        Assert.Equal(2, dbRefs.Count);
    }

    [Theory]
    [InlineData(ReferralStatus.Completed)]
    [InlineData(ReferralStatus.Accepted)]
    [InlineData(ReferralStatus.Expired)]
    public async Task NonValidStatusInviteExists_ReturnsNull(ReferralStatus status)
    {
        var dbRef = new Referral()
        {
            Id = Guid.NewGuid(),
            InvitedById = Guid.NewGuid(),
            InvitedEmail = "test@test.com",
            Status = status,
            InvitedAtUtc = new DateTime(1999, 1, 1)
        };
        await ReferralRepository.Store(dbRef);
        await ReferralRepository.SaveChanges();
        var referral = await _sut.Handle(new (dbRef.InvitedById, dbRef.InvitedEmail));
        
        _emailService.Verify(s => s.SendInviteEmail(It.IsAny<string>()), Times.Never);

        Assert.Null(referral);        
        var dbRefs = await ReferralRepository.Search(r => r.InvitedEmail == dbRef.InvitedEmail);
        Assert.Equal(1, dbRefs.Count);
    }

    public void Dispose()
    {
        var allReferrals = ReferralRepository.Search(t => true).Result;
        foreach (var referral in allReferrals)
        {
            ReferralRepository.Delete(referral).Wait();
        }

        ReferralRepository.SaveChanges().Wait();
    }
}