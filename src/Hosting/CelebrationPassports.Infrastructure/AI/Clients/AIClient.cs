using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using CelebrationPassports.Infrastructure.AI.Models;
using CelebrationPassports.Infrastructure.AI.Configuration;

namespace CelebrationPassports.Infrastructure.AI.Clients;

public class AIClient
{
    private readonly HttpClient _httpClient;
    private readonly AIOptions _options;

    public AIClient(HttpClient httpClient, IOptions<AIOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var request = new AIRequest
        {
            Model = _options.Model,
            Prompt = prompt,
            Stream = false
        };
        Console.WriteLine($"BaseUrl = {_options.BaseUrl}");

        var response = await _httpClient.PostAsJsonAsync(
            $"{_options.BaseUrl}/api/generate",
            request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AIResponse>();

        return result?.Response ?? string.Empty;
    }
}