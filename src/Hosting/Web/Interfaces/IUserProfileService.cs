using CelebrationPassports.Web.Models.Settings;

namespace CelebrationPassports.Web.Interfaces;

public interface IUserProfileService
{
    Task<SettingsViewModel> GetSettingsAsync();

    Task<bool> SetHomePlaceAsync(Guid? placeId);
}
