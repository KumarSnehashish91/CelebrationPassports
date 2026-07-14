using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Events.DTOs;

public class EventSummaryDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public EventStatus Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid? PlaceId { get; set; }
}
