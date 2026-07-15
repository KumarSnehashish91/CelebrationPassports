using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Ideas;

namespace CelebrationPassports.Web.Services;

public class SomedayIdeaService : ISomedayIdeaService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public SomedayIdeaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SomedayIdeaViewModel>> GetByPassportAsync(Guid passportId)
    {
        var response = await _httpClient.GetAsync($"api/passports/{passportId}/someday-ideas");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<IdeaBody>>(JsonOptions);

        return body?.Select(MapToViewModel).ToList() ?? [];
    }

    public async Task<bool> CreateAsync(Guid passportId, string title, string? notes)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/passports/{passportId}/someday-ideas", new { title, notes });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/someday-ideas/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<Guid?> ConvertToEventAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"api/someday-ideas/{id}/convert", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<IdeaBody>(JsonOptions);
        return body?.ConvertedToEventId;
    }

    private static SomedayIdeaViewModel MapToViewModel(IdeaBody body) => new()
    {
        Id = body.Id,
        PassportId = body.PassportId,
        Title = body.Title,
        Notes = body.Notes,
        CreatedAt = body.CreatedAt,
        ConvertedToEventId = body.ConvertedToEventId
    };

    private sealed class IdeaBody
    {
        public Guid Id { get; set; }
        public Guid PassportId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ConvertedToEventId { get; set; }
    }
}
