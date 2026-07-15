using CelebrationPassports.Application.Events;
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
    private readonly IGenericRepository<Expense> _expenseRepository;
    private readonly IGenericRepository<ExpenseCategoryBudget> _budgetRepository;
    private readonly IGenericRepository<Story> _storyRepository;
    private readonly IPassportRepository _passportRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateEventRequest> _createValidator;
    private readonly IValidator<UpdateEventRequest> _updateValidator;
    private readonly IValidator<AddCalendarEventRequest> _addCalendarEventValidator;

    public EventService(
        IEventRepository eventRepository,
        IGenericRepository<CalendarEvent> calendarEventRepository,
        IGenericRepository<Expense> expenseRepository,
        IGenericRepository<ExpenseCategoryBudget> budgetRepository,
        IGenericRepository<Story> storyRepository,
        IPassportRepository passportRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateEventRequest> createValidator,
        IValidator<UpdateEventRequest> updateValidator,
        IValidator<AddCalendarEventRequest> addCalendarEventValidator)
    {
        _eventRepository = eventRepository;
        _calendarEventRepository = calendarEventRepository;
        _expenseRepository = expenseRepository;
        _budgetRepository = budgetRepository;
        _storyRepository = storyRepository;
        _passportRepository = passportRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _addCalendarEventValidator = addCalendarEventValidator;
    }

    public async Task<IReadOnlyList<EventSummaryDto>> ListByPassportAsync(Guid userId, Guid passportId, EventStatus? status)
    {
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        // Upcoming/Ongoing/Completed aren't stored, they're computed on read (see
        // EventStatusCalculator) — so filtering has to happen after mapping, not at the
        // repository level, except for Draft which is a real stored value either way.
        var events = await _eventRepository.GetByPassportAsync(passportId, status == EventStatus.Draft ? status : null);

        var summaries = events.Select(MapToSummary);

        if (status.HasValue)
        {
            summaries = summaries.Where(e => e.Status == status.Value);
        }

        var result = summaries.ToList();
        await AttachBudgetTotalsAsync(result);

        return result;
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
            EventType = request.EventType,
            Status = EventStatus.Draft,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsAllDay = request.IsAllDay,
            TimeZoneId = request.TimeZoneId,
            PlaceId = request.PlaceId,
            CoverMediaId = request.CoverMediaId,
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

        // Evaluated against the event's CURRENT (pre-update) schedule, not the incoming
        // request — an event that's happening right now can't be edited, full stop.
        if (EventStatusCalculator.GetEffectiveStatus(@event, DateTime.UtcNow) == EventStatus.Ongoing)
        {
            throw new ConflictException("This event is ongoing and cannot be modified.");
        }

        @event.Title = request.Title;
        @event.EventType = request.EventType;
        // Ongoing/Completed are never persisted — they're computed on read from the
        // schedule (see EventStatusCalculator), so the only real stored states are
        // Draft (still mid-wizard) and Upcoming (published). Anything else the caller
        // sends collapses to Upcoming rather than writing a stale/misleading value.
        @event.Status = request.Status == EventStatus.Draft ? EventStatus.Draft : EventStatus.Upcoming;
        @event.StartDate = request.StartDate;
        @event.EndDate = request.EndDate;
        @event.StartTime = request.StartTime;
        @event.EndTime = request.EndTime;
        @event.IsAllDay = request.IsAllDay;
        @event.TimeZoneId = request.TimeZoneId;
        @event.PlaceId = request.PlaceId;
        @event.CoverMediaId = request.CoverMediaId;
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

    public async Task<EventDetailDto> CancelAsync(Guid userId, Guid eventId)
    {
        var @event = await _eventRepository.GetByIdWithCalendarEventsAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var effectiveStatus = EventStatusCalculator.GetEffectiveStatus(@event, DateTime.UtcNow);

        if (effectiveStatus is EventStatus.Ongoing or EventStatus.Completed or EventStatus.Cancelled)
        {
            throw new ConflictException("This event can't be cancelled right now.");
        }

        @event.Status = EventStatus.Cancelled;

        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(@event);
    }

    public async Task<EventDetailDto> LinkStoryAsync(Guid userId, Guid eventId, Guid storyId)
    {
        var @event = await _eventRepository.GetByIdWithCalendarEventsAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var story = await _storyRepository.GetByIdAsync(storyId)
            ?? throw new NotFoundException("Story not found.");

        if (story.PassportId != @event.PassportId)
        {
            throw new ValidationException("That story belongs to a different passport.");
        }

        @event.StoryId = storyId;

        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(@event);
    }

    public async Task<IReadOnlyList<EventSummaryDto>> GetUpcomingForUserAsync(Guid userId, int take)
    {
        var passports = await _passportRepository.GetForUserAsync(userId);
        var passportIds = passports.Select(p => p.Id).ToList();

        if (passportIds.Count == 0)
        {
            return [];
        }

        var events = await _eventRepository.GetUpcomingForPassportsAsync(
            passportIds, DateOnly.FromDateTime(DateTime.UtcNow));

        return events
            .Select(MapToSummary)
            .Where(e => e.Status == EventStatus.Upcoming)
            .OrderBy(e => e.StartDate)
            .ThenBy(e => e.StartTime)
            .Take(take)
            .ToList();
    }

    public async Task<IReadOnlyList<EventSummaryDto>> GetAllForUserAsync(Guid userId, EventStatus? status)
    {
        var passports = await _passportRepository.GetForUserAsync(userId);
        var passportIds = passports.Select(p => p.Id).ToList();

        if (passportIds.Count == 0)
        {
            return [];
        }

        // Upcoming/Ongoing/Completed aren't stored — fetch everything for a Draft-only
        // filter (a real stored value), otherwise fetch all and filter on computed status.
        var events = await _eventRepository.GetForPassportsAsync(passportIds, status == EventStatus.Draft ? status : null);

        var summaries = events.Select(MapToSummary);

        if (status.HasValue)
        {
            summaries = summaries.Where(e => e.Status == status.Value);
        }

        var result = summaries.ToList();
        await AttachBudgetTotalsAsync(result);

        return result;
    }

    // Two batched queries (all budgets/expenses for the visible events) instead of one
    // per event — the event list repository methods return already-materialized lists,
    // not IQueryable, so this can't be folded into the original DB query without a
    // repository change; batching here still keeps it at a fixed 2 extra round-trips
    // regardless of list size.
    private async Task AttachBudgetTotalsAsync(List<EventSummaryDto> summaries)
    {
        if (summaries.Count == 0)
        {
            return;
        }

        var eventIds = summaries.Select(s => s.Id).ToHashSet();

        var budgets = await _budgetRepository.FindAsync(b => eventIds.Contains(b.EventId));
        var expenses = await _expenseRepository.FindAsync(e => eventIds.Contains(e.EventId) && !e.IsDeleted);

        var budgetByEvent = budgets
            .GroupBy(b => b.EventId)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.BudgetedAmount));

        var spentByEvent = expenses
            .GroupBy(e => e.EventId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        foreach (var summary in summaries)
        {
            summary.TotalBudgeted = budgetByEvent.GetValueOrDefault(summary.Id);
            summary.TotalSpent = spentByEvent.GetValueOrDefault(summary.Id);
        }
    }

    private static EventSummaryDto MapToSummary(Event @event) => new()
    {
        Id = @event.Id,
        Title = @event.Title,
        EventType = @event.EventType,
        Status = EventStatusCalculator.GetEffectiveStatus(@event, DateTime.UtcNow),
        StartDate = @event.StartDate,
        EndDate = @event.EndDate,
        StartTime = @event.StartTime,
        IsAllDay = @event.IsAllDay,
        PlaceId = @event.PlaceId,
        CoverMediaId = @event.CoverMediaId
    };

    private static EventDetailDto MapToDetail(Event @event) => new()
    {
        Id = @event.Id,
        PassportId = @event.PassportId,
        Title = @event.Title,
        EventType = @event.EventType,
        Status = EventStatusCalculator.GetEffectiveStatus(@event, DateTime.UtcNow),
        StartDate = @event.StartDate,
        EndDate = @event.EndDate,
        StartTime = @event.StartTime,
        EndTime = @event.EndTime,
        IsAllDay = @event.IsAllDay,
        TimeZoneId = @event.TimeZoneId,
        PlaceId = @event.PlaceId,
        CoverMediaId = @event.CoverMediaId,
        Notes = @event.Notes,
        StoryId = @event.StoryId,
        CreatedAt = @event.CreatedAt,
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
