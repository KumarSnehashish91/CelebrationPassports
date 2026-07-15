using System.Net.Http.Json;
using CelebrationPassports.Web.Interfaces;

namespace CelebrationPassports.Web.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;

    public AIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GenerateAsync(string prompt)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ai/generate", prompt);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<string>();
    }
}
