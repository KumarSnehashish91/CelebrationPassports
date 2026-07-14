using CelebrationPassports.Application.Events.DTOs;
using CelebrationPassports.Application.Events.Interfaces;
using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Events.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IGenericRepository<CalendarEvent> _calendarEventRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateEventRequest> _createValidator;
    private readonly IValidator<UpdateEventRequest> _updateValidator;
    private readonly IValidator<AddCalendarEventRequest> _addCalendarEventValidator;

    public EventService(
        IEventRepository eventRepository,
        IGenericRepository<CalendarEvent> calendarEventRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateEventRequest> createValidator,
        IValidator<UpdateEventRequest> updateValidator,
        IValidator<AddCalendarEventRequest> addCalendarEventValidator)
    {
        _eventRepository = eventRepository;
        _calendarEventRepository = calendarEventRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _addCalendarEventValidator = addCalendarEventValidator;
    }

    public async Task<IReadOnlyList<EventSummaryDto>> ListByPassportAsync(Guid userId, Guid passportId, EventStatus? status)
    {
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var events = await _eventRepository.GetByPassportAsync(passportId, status);

        return events.Select(MapToSummary).ToList();
    }

    public async Task<EventDetailDto> GetByIdAsync(Guid userId, Guid eventId)
    {
        var @event = await _eventRepository.GetByIdWithCalendarEventsAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        return MapToDetail(@event);
    }

    public async Task<EventDetailDto> CreateAsync(Guid userId, Guid passportId, CreateEventRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var @event = new Event
        {
            Id = Guid.NewGuid(),
            PassportId = passportId,
            Title = request.Title,
            Status = EventStatus.Draft,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            PlaceId = request.PlaceId,
            Notes = request.Notes,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(@event);
    }

    public async Task<EventDetailDto> UpdateAsync(Guid userId, Guid eventId, UpdateEventRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var @event = await _eventRepository.GetByIdWithCalendarEventsAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        @event.Title = request.Title;
        @event.Status = request.Status;
        @event.StartDate = request.StartDate;
        @event.EndDate = request.EndDate;
        @event.PlaceId = request.PlaceId;
        @event.Notes = request.Notes;

        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(@event);
    }

    public async Task<EventDetailDto> AddCalendarEventAsync(Guid userId, Guid eventId, AddCalendarEventRequest request)
    {
        await _addCalendarEventValidator.ValidateAndThrowAsync(request);

        var @event = await _eventRepository.GetByIdWithCalendarEventsAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var calendarEvent = new CalendarEvent
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            CreatedBy = userId,
            Title = request.Title,
            Location = request.Location,
            EventTime = request.EventTime,
            ColorTag = request.ColorTag
        };

        await _calendarEventRepository.AddAsync(calendarEvent);
        await _unitOfWork.SaveChangesAsync();

        @event.CalendarEvents.Add(calendarEvent);

        return MapToDetail(@event);
    }

    private static EventSummaryDto MapToSummary(Event @event) => new()
    {
        Id = @event.Id,
        Title = @event.Title,
        Status = @event.Status,
        StartDate = @event.StartDate,
        EndDate = @event.EndDate,
        PlaceId = @event.PlaceId
    };

    private static EventDetailDto MapToDetail(Event @event) => new()
    {
        Id = @event.Id,
        PassportId = @event.PassportId,
        Title = @event.Title,
        Status = @event.Status,
        StartDate = @event.StartDate,
        EndDate = @event.EndDate,
        PlaceId = @event.PlaceId,
        Notes = @event.Notes,
        StoryId = @event.StoryId,
        CalendarEvents = @event.CalendarEvents.Select(c => new CalendarEventDto
        {
            Id = c.Id,
            Title = c.Title,
            Location = c.Location,
            EventTime = c.EventTime,
            ColorTag = c.ColorTag
        }).ToList()
    };
}
