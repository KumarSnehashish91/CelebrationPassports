using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Events.DTOs;

public class EventSummaryDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public EventType EventType { get; set; }

    public EventStatus Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public bool IsAllDay { get; set; }

    public Guid? PlaceId { get; set; }

    public Guid? CoverMediaId { get; set; }
}
