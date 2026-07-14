using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Places;
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

        return model;
    }

    public async Task<bool> SetHomePlaceAsync(Guid? placeId)
    {
        var response = await _httpClient.PutAsJsonAsync("api/UserControllerAPI/me/home-place", new { placeId });
        return response.IsSuccessStatusCode;
    }

    private sealed class ProfileBody
    {
        public Guid? HomePlaceId { get; set; }
    }
}
