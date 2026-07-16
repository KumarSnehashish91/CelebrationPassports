using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Gifting;

namespace CelebrationPassports.Web.Services;

public class PassportGiftService : IPassportGiftService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public PassportGiftService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PassportGiftViewModel?> PurchaseAsync(PurchaseGiftViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/gifts", new
        {
            model.RecipientName,
            model.RecipientEmail,
            model.GiftMessage,
            model.PassportTitle
        });

        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<PassportGiftViewModel>(JsonOptions)
            : null;
    }

    public async Task<List<PassportGiftViewModel>> GetMyPurchasesAsync()
    {
        var response = await _httpClient.GetAsync("api/gifts/mine");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<PassportGiftViewModel>>(JsonOptions);
        return body ?? [];
    }

    public async Task<GiftClaimInfoViewModel?> GetClaimInfoAsync(string code)
    {
        var response = await _httpClient.GetAsync($"api/gifts/claim/{Uri.EscapeDataString(code)}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var info = await response.Content.ReadFromJsonAsync<GiftClaimInfoViewModel>(JsonOptions);

        if (info is not null)
        {
            info.CoverPhotoUrl = ResolveUrl(info.CoverPhotoUrl);
        }

        return info;
    }

    public async Task<Guid?> ClaimAsync(string code)
    {
        var response = await _httpClient.PostAsync($"api/gifts/claim/{Uri.EscapeDataString(code)}", null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<ClaimBody>(JsonOptions);
        return body?.PassportId;
    }

    public async Task<GiftMessageViewModel?> GetMessageAsync(Guid passportId)
    {
        var response = await _httpClient.GetAsync($"api/gifts/passports/{passportId}/message");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var message = await response.Content.ReadFromJsonAsync<GiftMessageViewModel>(JsonOptions);

        if (message is not null)
        {
            message.MediaUrl = ResolveUrl(message.MediaUrl);
        }

        return message;
    }

    public async Task<ClaimedGiftSummaryViewModel?> GetClaimedGiftSummaryAsync(Guid passportId)
    {
        var response = await _httpClient.GetAsync($"api/gifts/passports/{passportId}/claimed-summary");

        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ClaimedGiftSummaryViewModel>(JsonOptions)
            : null;
    }

    // Photos/media are served by the API host, which differs from the Web app's own port.
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

    private sealed class ClaimBody
    {
        public Guid? PassportId { get; set; }
    }
}
