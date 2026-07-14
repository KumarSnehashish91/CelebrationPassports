using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<IReadOnlyList<Event>> GetByPassportAsync(Guid passportId, EventStatus? status);

    Task<Event?> GetByIdWithCalendarEventsAsync(Guid id);
}
