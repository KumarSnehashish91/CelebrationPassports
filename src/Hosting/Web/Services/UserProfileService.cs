using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Places;
using CelebrationPassports.Web.Models.Profile;
using CelebrationPassports.Web.Models.Settings;

namespace CelebrationPassports.Web.Services;

public class UserProfileService : IUserProfileService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IPlaceService _placeService;

    public UserProfileService(HttpClient httpClient, IPlaceService placeService)
    {
        _httpClient = httpClient;
        _placeService = placeService;
    }

    public async Task<SettingsViewModel> GetSettingsAsync()
    {
        var response = await _httpClient.GetAsync("api/UserControllerAPI/me");

        if (!response.IsSuccessStatusCode)
        {
            return new SettingsViewModel();
        }

        var body = await response.Content.ReadFromJsonAsync<ProfileBody>(JsonOptions);

        if (body is null)
        {
            return new SettingsViewModel();
        }

        var model = new SettingsViewModel { HomePlaceId = body.HomePlaceId };

        if (body.HomePlaceId.HasValue)
        {
            var place = await _placeService.GetByIdAsync(body.HomePlaceId.Value);
            model.HomePlaceName = place is null ? null : place.Name + (!string.IsNullOrWhiteSpace(place.City) ? $", {place.City}" : "");
        }

        model.CalendarFeedUrl = await GetCalendarFeedUrlAsync();

        return model;
    }

    private async Task<string?> GetCalendarFeedUrlAsync()
    {
        var response = await _httpClient.GetAsync("api/users/me/calendar-feed");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<CalendarFeedBody>(JsonOptions);

        if (body is null || _httpClient.BaseAddress is null)
        {
            return null;
        }

        // Path is API-relative (e.g. "/api/calendar/{userId}/{token}.ics") — resolve
        // against the API's own base address, same pattern as MediaService.GetUrlAsync,
        // since Web and API run on different ports/hosts.
        return new Uri(_httpClient.BaseAddress, body.Path).ToString();
    }

    public async Task<bool> SetHomePlaceAsync(Guid? placeId)
    {
        var response = await _httpClient.PutAsJsonAsync("api/UserControllerAPI/me/home-place", new { placeId });
        return response.IsSuccessStatusCode;
    }

    public async Task<ProfileViewModel?> GetProfileAsync()
    {
        var response = await _httpClient.GetAsync("api/UserControllerAPI/me");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<ProfileBody>(JsonOptions);

        if (body is null)
        {
            return null;
        }

        return new ProfileViewModel
        {
            FirstName = body.FirstName,
            LastName = body.LastName,
            DisplayName = body.DisplayName,
            DateOfBirth = body.DateOfBirth,
            Gender = body.Gender,
            MobileNumber = body.MobileNumber,
            ProfilePhotoUrl = body.ProfilePhotoUrl,
            CreatedOn = body.CreatedOn
        };
    }

    private sealed class CalendarFeedBody
    {
        public string Path { get; set; } = string.Empty;
    }

    private sealed class ProfileBody
    {
        public Guid? HomePlaceId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? MobileNumber { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
