namespace CelebrationPassports.Application.Events.DTOs;

public class CreateEventRequest
{
    public string Title { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid? PlaceId { get; set; }

    public string? Notes { get; set; }
}
