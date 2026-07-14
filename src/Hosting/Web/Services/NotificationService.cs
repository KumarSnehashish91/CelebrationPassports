using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Notifications;

namespace CelebrationPassports.Web.Services;

public class NotificationService : INotificationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public NotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<NotificationViewModel>> GetMineAsync(int take = 20)
    {
        var response = await _httpClient.GetAsync($"api/notifications/mine?take={take}");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<NotificationViewModel>>(JsonOptions);
        return body ?? [];
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var response = await _httpClient.GetAsync("api/notifications/mine/unread-count");

        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }

        var body = await response.Content.ReadFromJsonAsync<CountBody>(JsonOptions);
        return body?.Count ?? 0;
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        await _httpClient.PostAsync($"api/notifications/{id}/read", null);
    }

    private sealed class CountBody
    {
        public int Count { get; set; }
    }
}
