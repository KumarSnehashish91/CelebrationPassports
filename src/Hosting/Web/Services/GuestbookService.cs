using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Guestbook;

namespace CelebrationPassports.Web.Services;

public class GuestbookService : IGuestbookService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public GuestbookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetShareTokenAsync(Guid chapterId)
    {
        var response = await _httpClient.GetAsync($"api/chapters/{chapterId}/guestbook/link");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<TokenBody>(JsonOptions);
        return body?.Token;
    }

    public async Task<List<GuestbookSubmissionViewModel>> GetPendingAsync(Guid chapterId)
    {
        var response = await _httpClient.GetAsync($"api/chapters/{chapterId}/guestbook/pending");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<SubmissionBody>>(JsonOptions);

        return body?.Select(s => new GuestbookSubmissionViewModel
        {
            Id = s.Id,
            ChapterId = s.ChapterId,
            GuestName = s.GuestName,
            Message = s.Message,
            PhotoUrl = ResolveUrl(s.PhotoUrl),
            CreatedAt = s.CreatedAt
        }).ToList() ?? [];
    }

    public async Task<bool> ApproveAsync(Guid submissionId)
    {
        var response = await _httpClient.PostAsync($"api/guestbook/{submissionId}/approve", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RejectAsync(Guid submissionId)
    {
        var response = await _httpClient.PostAsync($"api/guestbook/{submissionId}/reject", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<GuestbookChapterInfoViewModel?> GetPublicInfoAsync(Guid chapterId, string token)
    {
        var response = await _httpClient.GetAsync($"api/guestbook/{chapterId}/info?token={Uri.EscapeDataString(token)}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<ChapterInfoBody>(JsonOptions);

        return body is null ? null : new GuestbookChapterInfoViewModel
        {
            ChapterTitle = body.ChapterTitle,
            EventDate = body.EventDate
        };
    }

    public async Task<(bool Success, string? Error)> SubmitAsync(Guid chapterId, string token, string guestName, string? message, IFormFile? photo)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(token), "Token" },
            { new StringContent(guestName), "GuestName" }
        };

        if (!string.IsNullOrWhiteSpace(message))
        {
            content.Add(new StringContent(message), "Message");
        }

        Stream? stream = null;

        try
        {
            if (photo is { Length: > 0 })
            {
                stream = photo.OpenReadStream();
                var streamContent = new StreamContent(stream);
                content.Add(streamContent, "Photo", photo.FileName);
            }

            var response = await _httpClient.PostAsync($"api/guestbook/{chapterId}/submit", content);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(error) ? "Could not submit — please try again." : error);
        }
        finally
        {
            if (stream is not null)
            {
                await stream.DisposeAsync();
            }
        }
    }

    // Same relative-to-API-host resolution IMediaService.GetUrlAsync uses — the API
    // serves uploads from its own host, which differs from the Web app's.
    private string? ResolveUrl(string? relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl))
        {
            return null;
        }

        return _httpClient.BaseAddress is null
            ? relativeUrl
            : new Uri(_httpClient.BaseAddress, relativeUrl).ToString();
    }

    private sealed class TokenBody
    {
        public string Token { get; set; } = string.Empty;
    }

    private sealed class SubmissionBody
    {
        public Guid Id { get; set; }
        public Guid ChapterId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private sealed class ChapterInfoBody
    {
        public string ChapterTitle { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
    }
}
