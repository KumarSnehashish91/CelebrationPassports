using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Sharing;

namespace CelebrationPassports.Web.Services;

public class ChapterSharingService : IChapterSharingService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public ChapterSharingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> InviteAsync(Guid chapterId, string email)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/chapters/{chapterId}/sharing/invite", new { email });
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ChapterInvitationViewModel>> GetPendingForMeAsync()
    {
        var response = await _httpClient.GetAsync("api/chapter-invitations/mine");
        return await ReadInvitationsAsync(response);
    }

    public async Task<List<ChapterInvitationViewModel>> GetByChapterAsync(Guid chapterId)
    {
        var response = await _httpClient.GetAsync($"api/chapters/{chapterId}/sharing/invitations");
        return await ReadInvitationsAsync(response);
    }

    public async Task<List<ChapterContributorViewModel>> GetContributorsAsync(Guid chapterId)
    {
        var response = await _httpClient.GetAsync($"api/chapters/{chapterId}/sharing/contributors");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<ContributorBody>>(JsonOptions);

        return body?.Select(c => new ChapterContributorViewModel
        {
            Id = c.Id,
            ChapterId = c.ChapterId,
            Name = c.Name,
            CreatedAt = c.CreatedAt
        }).ToList() ?? [];
    }

    public async Task<Guid?> AcceptAsync(Guid invitationId)
    {
        var response = await _httpClient.PostAsync($"api/chapter-invitations/{invitationId}/accept", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<AcceptBody>(JsonOptions);
        return body?.ChapterId;
    }

    public async Task<bool> DeclineAsync(Guid invitationId)
    {
        var response = await _httpClient.PostAsync($"api/chapter-invitations/{invitationId}/decline", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveContributorAsync(Guid contributorId)
    {
        var response = await _httpClient.DeleteAsync($"api/chapters/sharing/contributors/{contributorId}");
        return response.IsSuccessStatusCode;
    }

    private static async Task<List<ChapterInvitationViewModel>> ReadInvitationsAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<InvitationBody>>(JsonOptions);

        return body?.Select(i => new ChapterInvitationViewModel
        {
            Id = i.Id,
            ChapterId = i.ChapterId,
            ChapterTitle = i.ChapterTitle,
            Email = i.Email,
            Status = MapStatus(i.Status)
        }).ToList() ?? [];
    }

    // Mirrors CelebrationPassports.Persistence.Enums.ChapterInvitationStatus
    // (Pending=1, Accepted=2, Declined=3).
    private static string MapStatus(int status) => status switch
    {
        1 => "Pending",
        2 => "Accepted",
        3 => "Declined",
        _ => "Unknown"
    };

    private sealed class InvitationBody
    {
        public Guid Id { get; set; }
        public Guid ChapterId { get; set; }
        public string ChapterTitle { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Status { get; set; }
    }

    private sealed class AcceptBody
    {
        public Guid ChapterId { get; set; }
    }

    private sealed class ContributorBody
    {
        public Guid Id { get; set; }
        public Guid ChapterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
