using CelebrationPassports.Application.TripItinerary.DTOs;

namespace CelebrationPassports.Application.TripItinerary.Interfaces;

public interface ITripItineraryService
{
    // Replaces whatever itinerary the event already has — this is a "(re)generate",
    // not an incremental append, so a stale Day 3 from a previous plan can never survive
    // a new save with fewer days.
    Task<IReadOnlyList<TripItineraryDayDto>> SaveItineraryAsync(Guid userId, Guid eventId, SaveItineraryRequest request);

    Task<IReadOnlyList<TripItineraryDayDto>> GetByEventAsync(Guid userId, Guid eventId);
}
