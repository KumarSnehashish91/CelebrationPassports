namespace CelebrationPassports.Persistence.Entities;

public class TripItineraryDay
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public int DayNumber { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;
}
