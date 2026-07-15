using CelebrationPassports.Web.Models.TripPlanner;

namespace CelebrationPassports.Web.Interfaces;

public interface ITripItineraryService
{
    Task<List<TripPlanDayViewModel>> GetByEventAsync(Guid eventId);

    Task<bool> SaveItineraryAsync(Guid eventId, List<TripPlanDayViewModel> days);
}
