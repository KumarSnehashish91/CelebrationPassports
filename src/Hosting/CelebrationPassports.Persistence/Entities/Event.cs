using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class Event
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public EventType EventType { get; set; }

    public EventStatus Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsAllDay { get; set; }

    public string? TimeZoneId { get; set; }

    public Guid? PlaceId { get; set; }

    public Guid? CoverMediaId { get; set; }

    public string? Notes { get; set; }

    // Set once the event is completed and its Story is generated.
    public Guid? StoryId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual Place? Place { get; set; }

    public virtual Media? CoverMedia { get; set; }

    public virtual Story? Story { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
}
