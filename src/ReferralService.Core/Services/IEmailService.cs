namespace ReferralService.Core.Services;

public interface IEmailService
{
    Task SendInviteEmail(string to);
}