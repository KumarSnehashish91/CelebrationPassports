using CelebrationPassports.Web.Models.Dashboard;
using CelebrationPassports.Web.Models.Events;

namespace CelebrationPassports.Web.Interfaces;

public interface IEventService
{
    Task<List<UpcomingCelebration>> GetUpcomingAsync(int take = 5);

    Task<List<CelebrationListItemViewModel>> GetAllAsync(int? status = null);

    Task<EventWizardViewModel?> GetByIdAsync(Guid id);

    // Creates the event (model.Id is null) or updates it (model.Id is set) — returns the
    // event's id either way, or null on failure. The API's PUT is a full replace, so the
    // caller must have already merged this step's changes into a freshly-loaded model.
    Task<Guid?> SaveAsync(EventWizardViewModel model);

    Task<bool> FinalizeAsync(Guid id);

    Task<bool> AddCalendarEventAsync(Guid eventId, CalendarEventViewModel model);

    Task<bool> CancelAsync(Guid id);
}
