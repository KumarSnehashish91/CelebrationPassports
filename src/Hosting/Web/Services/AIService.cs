using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;

namespace CelebrationPassports.Web.Services;

public class AIService : IAIService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public AIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GenerateAsync(string prompt, int? maxTokens = null)
    {
        var url = maxTokens is null ? "api/ai/generate" : $"api/ai/generate?maxTokens={maxTokens}";
        var response = await _httpClient.PostAsJsonAsync(url, prompt);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<GenerateResponseBody>(JsonOptions);
        return body?.Response;
    }

    private sealed class GenerateResponseBody
    {
        public string? Response { get; set; }
    }
}
