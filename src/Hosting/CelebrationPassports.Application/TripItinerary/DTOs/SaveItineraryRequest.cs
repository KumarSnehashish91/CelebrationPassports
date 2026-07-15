namespace CelebrationPassports.Application.TripItinerary.DTOs;

public class ItineraryDayInput
{
    public int DayNumber { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class SaveItineraryRequest
{
    public List<ItineraryDayInput> Days { get; set; } = new();
}
