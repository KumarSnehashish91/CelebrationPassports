using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Stories;

namespace CelebrationPassports.Web.Services;

public class WishService : IWishService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public WishService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<WishViewModel>> GetByChapterAsync(Guid chapterId)
    {
        var response = await _httpClient.GetAsync($"api/chapters/{chapterId}/wishes");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<WishBody>>(JsonOptions);

        return body?.Select(w => new WishViewModel
        {
            Id = w.Id,
            ChapterId = w.ChapterId,
            UserId = w.UserId,
            AuthorName = w.AuthorName,
            Text = w.Text,
            CreatedAt = w.CreatedAt
        }).ToList() ?? [];
    }

    public async Task<bool> CreateAsync(Guid chapterId, string text)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/chapters/{chapterId}/wishes", new { text });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/wishes/{id}");
        return response.IsSuccessStatusCode;
    }

    private sealed class WishBody
    {
        public Guid Id { get; set; }
        public Guid ChapterId { get; set; }
        public Guid UserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
