using CelebrationPassports.Infrastructure.AI.Interfaces;
using CelebrationPassports.Infrastructure.AI.Clients;
using System.Threading.Tasks;

namespace CelebrationPassports.Infrastructure.AI.Services;

public class CelebrationAIService : ICelebrationAIService
{
    private readonly AIClient _aiClient;

    public CelebrationAIService(AIClient aiClient)
    {
        _aiClient = aiClient;
    }

    public async Task<string> GenerateAsync(string prompt, int? maxTokens = null)
    {
        return await _aiClient.GenerateAsync(prompt, maxTokens);
    }

    public async Task<string> GenerateWithImageAsync(string prompt, byte[] imageBytes, int? maxTokens = null, double? temperature = null)
    {
        return await _aiClient.GenerateWithImageAsync(prompt, imageBytes, maxTokens, temperature);
    }
}