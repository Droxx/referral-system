namespace ReferralService.Core.Services;

public class EmailService(HttpClient http) : IEmailService
{
    public Task SendInviteEmail(string to)
    {
        // Stub implementation. It is assumed that we will call an external email service here
        // Using the http client
        return Task.CompletedTask;
    }
    
    public class EmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}