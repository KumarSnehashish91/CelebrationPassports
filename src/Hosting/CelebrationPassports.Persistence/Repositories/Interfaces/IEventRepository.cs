using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<IReadOnlyList<Event>> GetByPassportAsync(Guid passportId, EventStatus? status);

    Task<Event?> GetByIdWithCalendarEventsAsync(Guid id);

    // A coarse pre-filter only (StartDate >= from, not Draft) — the caller still has to
    // compute exact Upcoming/Ongoing/Completed status against current date+time, so this
    // deliberately doesn't take/limit: that happens after the precise filter, not before.
    Task<IReadOnlyList<Event>> GetUpcomingForPassportsAsync(IEnumerable<Guid> passportIds, DateOnly from);

    Task<IReadOnlyList<Event>> GetForPassportsAsync(IEnumerable<Guid> passportIds, EventStatus? status);
}
