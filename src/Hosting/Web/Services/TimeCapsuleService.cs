using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.TimeCapsule;

namespace CelebrationPassports.Web.Services;

public class TimeCapsuleService : ITimeCapsuleService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public TimeCapsuleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TimeCapsuleMessageViewModel>> GetByPassportAsync(Guid passportId)
    {
        var response = await _httpClient.GetAsync($"api/passports/{passportId}/time-capsule-messages");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<MessageBody>>(JsonOptions);

        return body?.Select(m => new TimeCapsuleMessageViewModel
        {
            Id = m.Id,
            PassportId = m.PassportId,
            AuthorUserId = m.AuthorUserId,
            Title = m.Title,
            Content = m.Content,
            UnlockDate = m.UnlockDate,
            IsUnlocked = m.IsUnlocked,
            CreatedAt = m.CreatedAt
        }).ToList() ?? [];
    }

    public async Task<bool> CreateAsync(Guid passportId, string title, string content, DateTime unlockDate)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/passports/{passportId}/time-capsule-messages", new { title, content, unlockDate });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/time-capsule-messages/{id}");
        return response.IsSuccessStatusCode;
    }

    private sealed class MessageBody
    {
        public Guid Id { get; set; }
        public Guid PassportId { get; set; }
        public Guid AuthorUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime UnlockDate { get; set; }
        public bool IsUnlocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
