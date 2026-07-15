using CelebrationPassports.Web.Models.Passports;

namespace CelebrationPassports.Web.Models.Settings;

public class SettingsViewModel
{
    public Guid? HomePlaceId { get; set; }

    public string? HomePlaceName { get; set; }

    // Manual entry, same pattern as the Story/Event location forms — no live map/search
    // widget or geocoding service, so unlike those forms this also asks for coordinates
    // directly: trip detection is meaningless without real lat/lng to measure distance
    // from.
    public string? PlaceName { get; set; }

    public string? City { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? CalendarFeedUrl { get; set; }

    public List<PassportListItemViewModel> Passports { get; set; } = new();
}
