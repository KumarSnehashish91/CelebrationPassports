using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.TripItinerary.DTOs;
using CelebrationPassports.Application.TripItinerary.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.TripItinerary.Services;

public class TripItineraryService : ITripItineraryService
{
    private readonly IGenericRepository<TripItineraryDay> _itineraryRepository;
    private readonly IGenericRepository<Event> _eventRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SaveItineraryRequest> _saveValidator;

    public TripItineraryService(
        IGenericRepository<TripItineraryDay> itineraryRepository,
        IGenericRepository<Event> eventRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<SaveItineraryRequest> saveValidator)
    {
        _itineraryRepository = itineraryRepository;
        _eventRepository = eventRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _saveValidator = saveValidator;
    }

    public async Task<IReadOnlyList<TripItineraryDayDto>> SaveItineraryAsync(Guid userId, Guid eventId, SaveItineraryRequest request)
    {
        await _saveValidator.ValidateAndThrowAsync(request);

        var @event = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var existing = await _itineraryRepository.FindAsync(d => d.EventId == eventId);

        if (existing.Count > 0)
        {
            _itineraryRepository.RemoveRange(existing);
        }

        var days = request.Days
            .OrderBy(d => d.DayNumber)
            .Select(d => new TripItineraryDay
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                DayNumber = d.DayNumber,
                Title = d.Title,
                Description = d.Description,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        await _itineraryRepository.AddRangeAsync(days);
        await _unitOfWork.SaveChangesAsync();

        return days.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TripItineraryDayDto>> GetByEventAsync(Guid userId, Guid eventId)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event not found.");

        await _accessGuard.EnsureMemberAsync(userId, @event.PassportId);

        var days = await _itineraryRepository.FindAsync(d => d.EventId == eventId);

        return days.OrderBy(d => d.DayNumber).Select(MapToDto).ToList();
    }

    private static TripItineraryDayDto MapToDto(TripItineraryDay day) => new()
    {
        Id = day.Id,
        EventId = day.EventId,
        DayNumber = day.DayNumber,
        Title = day.Title,
        Description = day.Description
    };
}
