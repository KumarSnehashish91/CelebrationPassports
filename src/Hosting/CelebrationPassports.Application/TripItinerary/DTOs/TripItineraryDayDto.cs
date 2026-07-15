namespace CelebrationPassports.Application.TripItinerary.DTOs;

public class TripItineraryDayDto
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public int DayNumber { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
