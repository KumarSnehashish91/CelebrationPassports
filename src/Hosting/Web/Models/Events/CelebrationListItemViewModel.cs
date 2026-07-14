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
}
