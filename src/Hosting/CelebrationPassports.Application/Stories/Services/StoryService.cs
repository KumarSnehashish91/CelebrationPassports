using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.Stories.DTOs;
using CelebrationPassports.Application.Stories.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Stories.Services;

public class StoryService : IStoryService
{
    private readonly IStoryRepository _storyRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateStoryRequest> _createValidator;

    public StoryService(
        IStoryRepository storyRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateStoryRequest> createValidator)
    {
        _storyRepository = storyRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<IReadOnlyList<StorySummaryDto>> ListByPassportAsync(Guid userId, Guid passportId)
    {
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var stories = await _storyRepository.GetByPassportAsync(passportId);

        return stories.Select(MapToSummary).ToList();
    }

    public async Task<StoryDetailDto> GetByIdAsync(Guid userId, Guid storyId)
    {
        var story = await _storyRepository.GetByIdWithChaptersAsync(storyId)
            ?? throw new NotFoundException("Story not found.");

        await _accessGuard.EnsureMemberAsync(userId, story.PassportId);

        return MapToDetail(story);
    }

    public async Task<StoryDetailDto> CreateAsync(Guid userId, Guid passportId, CreateStoryRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var story = new Story
        {
            Id = Guid.NewGuid(),
            PassportId = passportId,
            Title = request.Title,
            PlaceId = request.PlaceId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DisplayOrder = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _storyRepository.AddAsync(story);
        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(story);
    }

    private static StorySummaryDto MapToSummary(Story story) => new()
    {
        Id = story.Id,
        Title = story.Title,
        PlaceId = story.PlaceId,
        StartDate = story.StartDate,
        EndDate = story.EndDate,
        DisplayOrder = story.DisplayOrder
    };

    private static StoryDetailDto MapToDetail(Story story) => new()
    {
        Id = story.Id,
        PassportId = story.PassportId,
        Title = story.Title,
        PlaceId = story.PlaceId,
        StartDate = story.StartDate,
        EndDate = story.EndDate,
        CoverMediaId = story.CoverMediaId,
        DisplayOrder = story.DisplayOrder,
        Chapters = story.Chapters.Select(c => new ChapterSummaryDto
        {
            Id = c.Id,
            Title = c.Title,
            CategoryId = c.CategoryId,
            PlaceId = c.PlaceId,
            CoverMediaId = c.CoverMediaId,
            EventDate = c.EventDate,
            DisplayOrder = c.DisplayOrder
        }).ToList()
    };
}
