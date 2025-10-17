using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ReferralService.Core.Settings;

namespace ReferralService.Core.Services;

public class CreditService(
    ILogger<CreditService> logger,
    ServiceUrisSettings settings,
    HttpClient http) : ICreditService
{
    public async Task MutateCredits(Guid userId, int amount, string reason)
    {
        logger.LogInformation("Calling CreditService to mutate credits for UserId: {UserId}, Amount: {Amount}, Reason: {Reason}",
            userId, amount, reason);
        
        var creditMutationRequest = new CreditMutationRequest
        {
            UserId = userId,
            Amount = amount,
            Reason = reason
        };

        var request = new HttpRequestMessage(HttpMethod.Post, new Uri(settings.CreditService, "transactions"));

        request.Content = JsonContent.Create(creditMutationRequest);

        try
        {
            await http.SendAsync(request);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }
    
    private class CreditMutationRequest
    {
        public required Guid UserId { get; set; }
        public required string Reason { get; set; }
        public required int Amount { get; set; }
    }
}