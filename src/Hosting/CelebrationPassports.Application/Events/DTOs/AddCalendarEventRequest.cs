namespace CelebrationPassports.Application.Events.DTOs;

public class AddCalendarEventRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Location { get; set; }

    public DateTime EventTime { get; set; }

    public string ColorTag { get; set; } = string.Empty;
}
