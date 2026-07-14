using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Repositories.Implementations;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(CelebrationPassportsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Event>> GetByPassportAsync(Guid passportId, EventStatus? status)
    {
        var query = _dbcontext.Events
            .AsNoTracking()
            .Where(e => e.PassportId == passportId && !e.IsDeleted);

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        return await query
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdWithCalendarEventsAsync(Guid id)
    {
        return await _dbcontext.Events
            .Include(e => e.CalendarEvents)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<IReadOnlyList<Event>> GetUpcomingForPassportsAsync(IEnumerable<Guid> passportIds, DateOnly from)
    {
        return await _dbcontext.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.Status != EventStatus.Draft
                && passportIds.Contains(e.PassportId) && e.StartDate >= from)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Event>> GetForPassportsAsync(IEnumerable<Guid> passportIds, EventStatus? status)
    {
        var query = _dbcontext.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted && passportIds.Contains(e.PassportId));

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}
