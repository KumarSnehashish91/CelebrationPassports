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

    public async Task<string> GenerateAsync(string prompt)
    {
        return await _aiClient.GenerateAsync(prompt);
    }
}