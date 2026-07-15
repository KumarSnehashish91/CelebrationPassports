using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.Someday.DTOs;
using CelebrationPassports.Application.Someday.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Someday.Services;

// Feature: Someday List (feature-backlog-v1.1.md, PLAN #13). Simple CRUD, no AI
// involvement, per spec.
public class SomedayIdeaService : ISomedayIdeaService
{
    private readonly IGenericRepository<SomedayIdea> _ideaRepository;
    private readonly IGenericRepository<Event> _eventRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateSomedayIdeaRequest> _createValidator;
    private readonly IValidator<UpdateSomedayIdeaRequest> _updateValidator;

    public SomedayIdeaService(
        IGenericRepository<SomedayIdea> ideaRepository,
        IGenericRepository<Event> eventRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateSomedayIdeaRequest> createValidator,
        IValidator<UpdateSomedayIdeaRequest> updateValidator)
    {
        _ideaRepository = ideaRepository;
        _eventRepository = eventRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<SomedayIdeaDto> CreateAsync(Guid userId, Guid passportId, CreateSomedayIdeaRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var idea = new SomedayIdea
        {
            Id = Guid.NewGuid(),
            PassportId = passportId,
            Title = request.Title,
            Notes = request.Notes,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _ideaRepository.AddAsync(idea);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(idea);
    }

    public async Task<IReadOnlyList<SomedayIdeaDto>> GetByPassportAsync(Guid userId, Guid passportId)
    {
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var ideas = await _ideaRepository.FindAsync(i => i.PassportId == passportId && !i.IsDeleted);

        return ideas
            .OrderByDescending(i => i.CreatedAt)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<SomedayIdeaDto> UpdateAsync(Guid userId, Guid ideaId, UpdateSomedayIdeaRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var idea = await _ideaRepository.GetByIdAsync(ideaId)
            ?? throw new NotFoundException("Idea not found.");

        if (idea.IsDeleted)
        {
            throw new NotFoundException("Idea not found.");
        }

        await _accessGuard.EnsureMemberAsync(userId, idea.PassportId);

        idea.Title = request.Title;
        idea.Notes = request.Notes;

        await _unitOfWork.SaveChangesAsync();

        return MapToDto(idea);
    }

    public async Task DeleteAsync(Guid userId, Guid ideaId)
    {
        var idea = await _ideaRepository.GetByIdAsync(ideaId)
            ?? throw new NotFoundException("Idea not found.");

        if (idea.IsDeleted)
        {
            return;
        }

        await _accessGuard.EnsureMemberAsync(userId, idea.PassportId);

        idea.IsDeleted = true;
        idea.DeletedOn = DateTime.UtcNow;
        idea.DeletedBy = userId;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<SomedayIdeaDto> ConvertToEventAsync(Guid userId, Guid ideaId)
    {
        var idea = await _ideaRepository.GetByIdAsync(ideaId)
            ?? throw new NotFoundException("Idea not found.");

        if (idea.IsDeleted)
        {
            throw new NotFoundException("Idea not found.");
        }

        await _accessGuard.EnsureMemberAsync(userId, idea.PassportId);

        if (idea.ConvertedToEventId.HasValue)
        {
            throw new ConflictException("This idea has already been converted to an event.");
        }

        var @event = new Event
        {
            Id = Guid.NewGuid(),
            PassportId = idea.PassportId,
            Title = idea.Title,
            // The idea doesn't carry a celebration type — the user picks one on the
            // Events wizard's first step, which this redirects into.
            EventType = EventType.Other,
            Status = EventStatus.Draft,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Notes = idea.Notes,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(@event);

        idea.ConvertedToEventId = @event.Id;

        await _unitOfWork.SaveChangesAsync();

        return MapToDto(idea);
    }

    private static SomedayIdeaDto MapToDto(SomedayIdea idea) => new()
    {
        Id = idea.Id,
        PassportId = idea.PassportId,
        Title = idea.Title,
        Notes = idea.Notes,
        CreatedByUserId = idea.CreatedByUserId,
        CreatedAt = idea.CreatedAt,
        ConvertedToEventId = idea.ConvertedToEventId
    };
}
