namespace CelebrationPassports.Persistence.Entities;

public class CalendarEvent
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public Guid CreatedBy { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Location { get; set; }

    public DateTime EventTime { get; set; }

    public string ColorTag { get; set; } = string.Empty;

    public virtual Event Event { get; set; } = null!;

    public virtual User CreatedByUser { get; set; } = null!;
}
