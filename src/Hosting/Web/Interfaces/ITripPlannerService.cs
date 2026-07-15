using CelebrationPassports.Web.Models.TripPlanner;

namespace CelebrationPassports.Web.Interfaces;

public interface ITripPlannerService
{
    Task<TripPlanViewModel?> GenerateAsync(string destination, int days, string? notes);
}
