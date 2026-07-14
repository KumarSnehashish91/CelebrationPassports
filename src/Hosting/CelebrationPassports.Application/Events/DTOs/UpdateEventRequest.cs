using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Events.DTOs;

public class UpdateEventRequest
{
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
}
