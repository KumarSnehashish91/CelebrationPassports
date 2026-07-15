using CelebrationPassports.Application.Events.DTOs;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Events.Interfaces;

public interface IEventService
{
    Task<IReadOnlyList<EventSummaryDto>> ListByPassportAsync(Guid userId, Guid passportId, EventStatus? status);

    Task<EventDetailDto> GetByIdAsync(Guid userId, Guid eventId);

    Task<EventDetailDto> CreateAsync(Guid userId, Guid passportId, CreateEventRequest request);

    Task<EventDetailDto> UpdateAsync(Guid userId, Guid eventId, UpdateEventRequest request);

    Task<EventDetailDto> AddCalendarEventAsync(Guid userId, Guid eventId, AddCalendarEventRequest request);

    Task<EventDetailDto> CancelAsync(Guid userId, Guid eventId);

    Task<IReadOnlyList<EventSummaryDto>> GetUpcomingForUserAsync(Guid userId, int take);

    Task<IReadOnlyList<EventSummaryDto>> GetAllForUserAsync(Guid userId, EventStatus? status);
}
