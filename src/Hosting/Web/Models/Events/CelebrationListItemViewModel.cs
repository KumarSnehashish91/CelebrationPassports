namespace CelebrationPassports.Web.Models.Events;

public class CelebrationListItemViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int EventType { get; set; }

    public int Status { get; set; }

    public DateOnly StartDate { get; set; }

    // Events have no cover image resolution yet — reuse the existing placeholder
    // used elsewhere on the dashboard until Media/cover-image URL wiring lands.
    public string ImageUrl { get; set; } = "/images/udaipur.jpg";

    public string? PlaceName { get; set; }

    public decimal TotalBudgeted { get; set; }

    public decimal TotalSpent { get; set; }

    // Mirrors CelebrationPassports.Persistence.Enums.EventStatus (Draft=1, Upcoming=2,
    // Ongoing=3, Completed=4, Cancelled=5).
    public int CountdownDays => Math.Abs((StartDate.ToDateTime(TimeOnly.MinValue).Date - DateTime.Today).Days);

    public bool IsPast => Status is 4;
}
