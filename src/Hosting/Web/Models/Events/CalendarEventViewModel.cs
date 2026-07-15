namespace CelebrationPassports.Web.Models.Events;

public class CalendarEventViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Location { get; set; }

    public DateTime EventTime { get; set; }

    public string ColorTag { get; set; } = string.Empty;
}
