using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Gifting;
using CelebrationPassports.Web.Models.GiftStories;

namespace CelebrationPassports.Web.Services;

public class GiftStoryService : IGiftStoryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public GiftStoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GiftDraftViewModel?> StartAsync(PurchaseGiftViewModel recipientInfo)
    {
        var response = await _httpClient.PostAsJsonAsync("api/gift-stories", new
        {
            recipientInfo.RecipientName,
            recipientInfo.RecipientEmail,
            recipientInfo.GiftMessage,
            recipientInfo.PassportTitle
        });

        return response.IsSuccessStatusCode ? await ReadDraftAsync(response) : null;
    }

    public async Task<GiftDraftViewModel?> GetAsync(Guid draftId)
    {
        var response = await _httpClient.GetAsync($"api/gift-stories/{draftId}");
        return response.IsSuccessStatusCode ? await ReadDraftAsync(response) : null;
    }

    public async Task<GiftPhotoViewModel?> AddPhotoAsync(Guid draftId, IFormFile file)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var streamContent = new StreamContent(stream);
        content.Add(streamContent, "file", file.FileName);

        var response = await _httpClient.PostAsync($"api/gift-stories/{draftId}/photos", content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var photo = await response.Content.ReadFromJsonAsync<GiftPhotoViewModel>(JsonOptions);

        if (photo is not null)
        {
            photo.Url = ResolveUrl(photo.Url) ?? photo.Url;
        }

        return photo;
    }

    public async Task<bool> RemovePhotoAsync(Guid photoId)
    {
        var response = await _httpClient.DeleteAsync($"api/gift-stories/photos/{photoId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<GiftDraftViewModel?> ContinueToInsightsAsync(Guid draftId)
    {
        var response = await _httpClient.PostAsync($"api/gift-stories/{draftId}/continue-to-insights", null);
        return response.IsSuccessStatusCode ? await ReadDraftAsync(response) : null;
    }

    public async Task<bool> SetInsightsAsync(Guid draftId, Dictionary<Guid, string?> insights)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/gift-stories/{draftId}/insights", insights);
        return response.IsSuccessStatusCode;
    }

    public async Task<GeneratedStoryViewModel?> GenerateStoryAsync(Guid draftId)
    {
        var response = await _httpClient.PostAsync($"api/gift-stories/{draftId}/generate", null);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<GeneratedStoryViewModel>(JsonOptions) : null;
    }

    public async Task<GeneratedStoryViewModel?> RegenerateStoryAsync(Guid draftId)
    {
        var response = await _httpClient.PostAsync($"api/gift-stories/{draftId}/regenerate", null);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<GeneratedStoryViewModel>(JsonOptions) : null;
    }

    public async Task<GeneratedStoryViewModel?> GetStoryAsync(Guid draftId)
    {
        var response = await _httpClient.GetAsync($"api/gift-stories/{draftId}/story");
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<GeneratedStoryViewModel>(JsonOptions) : null;
    }

    public async Task<PrintPreviewViewModel?> GetPrintPreviewAsync(Guid draftId, string format)
    {
        var response = await _httpClient.GetAsync($"api/gift-stories/{draftId}/print-preview?format={Uri.EscapeDataString(format)}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var preview = await response.Content.ReadFromJsonAsync<PrintPreviewViewModel>(JsonOptions);

        if (preview is not null)
        {
            foreach (var page in preview.Pages)
            {
                page.PhotoUrl = ResolveUrl(page.PhotoUrl);
            }
        }

        return preview;
    }

    public async Task<GiftDraftViewModel?> SetMessageAsync(Guid draftId, string messageType, IFormFile? media, string? writtenText)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(messageType), "MessageType" }
        };

        if (!string.IsNullOrWhiteSpace(writtenText))
        {
            content.Add(new StringContent(writtenText), "WrittenText");
        }

        Stream? stream = null;

        try
        {
            if (media is { Length: > 0 })
            {
                stream = media.OpenReadStream();
                var streamContent = new StreamContent(stream);
                content.Add(streamContent, "Media", media.FileName);
            }

            var response = await _httpClient.PostAsync($"api/gift-stories/{draftId}/message", content);
            return response.IsSuccessStatusCode ? await ReadDraftAsync(response) : null;
        }
        finally
        {
            if (stream is not null)
            {
                await stream.DisposeAsync();
            }
        }
    }

    public async Task<GiftDraftViewModel?> SetDeliveryScheduleAsync(Guid draftId, string mode, DateTime? scheduledDate)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/gift-stories/{draftId}/schedule", new { Mode = mode, ScheduledDate = scheduledDate });
        return response.IsSuccessStatusCode ? await ReadDraftAsync(response) : null;
    }

    public async Task<PassportGiftViewModel?> FinalizeAsync(Guid draftId, string format)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/gift-stories/{draftId}/finalize", new { Format = format });
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<PassportGiftViewModel>(JsonOptions) : null;
    }

    private async Task<GiftDraftViewModel?> ReadDraftAsync(HttpResponseMessage response)
    {
        var draft = await response.Content.ReadFromJsonAsync<GiftDraftViewModel>(JsonOptions);

        if (draft is not null)
        {
            foreach (var photo in draft.Photos)
            {
                photo.Url = ResolveUrl(photo.Url) ?? photo.Url;
            }

            draft.MessageMediaUrl = ResolveUrl(draft.MessageMediaUrl);
        }

        return draft;
    }

    // Photos are served by the API host, which differs from the Web app's own port.
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
}
