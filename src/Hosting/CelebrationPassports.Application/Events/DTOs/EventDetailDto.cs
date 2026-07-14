using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Events.DTOs;

public class EventDetailDto
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    public EventStatus Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid? PlaceId { get; set; }

    public string? Notes { get; set; }

    public Guid? StoryId { get; set; }

    public List<CalendarEventDto> CalendarEvents { get; set; } = new();
}
