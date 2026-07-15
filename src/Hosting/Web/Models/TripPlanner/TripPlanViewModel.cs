using System.ComponentModel.DataAnnotations;

namespace CelebrationPassports.Web.Models.TripPlanner;

public class TripPlanViewModel
{
    [Required(ErrorMessage = "Destination is required.")]
    [StringLength(150)]
    [Display(Name = "Destination")]
    public string Destination { get; set; } = string.Empty;

    [Range(1, 14, ErrorMessage = "Trips can be planned for 1 to 14 days.")]
    [Display(Name = "Number of Days")]
    public int Days { get; set; } = 3;

    [StringLength(300)]
    [Display(Name = "Preferences (optional)")]
    public string? Notes { get; set; }

    // Populated after generation.
    public string? Overview { get; set; }

    public List<TripPlanDayViewModel> Itinerary { get; set; } = [];

    public List<string> PhotoUrls { get; set; } = [];

    public bool HasResult => !string.IsNullOrWhiteSpace(Overview) || Itinerary.Count > 0;

    // Folded into the saved Event's Notes field (max 2000 chars there) when the user
    // saves this plan as a trip — capped defensively so a long itinerary can't trip that
    // limit.
    public string BuildSavableDescription()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(Overview))
        {
            parts.Add(Overview.Trim());
        }

        parts.AddRange(Itinerary
            .OrderBy(d => d.DayNumber)
            .Select(d => $"Day {d.DayNumber}: {d.Title} — {d.Description}"));

        var combined = string.Join("\n", parts);

        return combined.Length > 1900 ? combined[..1900] : combined;
    }
}
