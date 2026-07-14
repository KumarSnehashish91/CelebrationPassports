using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.Stories.DTOs;
using CelebrationPassports.Application.Stories.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Stories.Services;

public class ChapterService : IChapterService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IStoryRepository _storyRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateChapterRequest> _createValidator;
    private readonly IValidator<UpdateChapterRequest> _updateValidator;

    public ChapterService(
        IChapterRepository chapterRepository,
        IStoryRepository storyRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateChapterRequest> createValidator,
        IValidator<UpdateChapterRequest> updateValidator)
    {
        _chapterRepository = chapterRepository;
        _storyRepository = storyRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ChapterDetailDto> GetByIdAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.Story.PassportId);

        return MapToDetail(chapter);
    }

    public async Task<ChapterDetailDto> CreateAsync(Guid userId, Guid storyId, CreateChapterRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var story = await _storyRepository.GetByIdAsync(storyId)
            ?? throw new NotFoundException("Story not found.");

        await _accessGuard.EnsureMemberAsync(userId, story.PassportId);

        var chapter = new Chapter
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            CategoryId = request.CategoryId,
            PlaceId = request.PlaceId,
            Title = request.Title,
            EventDate = request.EventDate,
            DisplayOrder = 0
        };

        await _chapterRepository.AddAsync(chapter);
        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(chapter);
    }

    public async Task<ChapterDetailDto> UpdateAsync(Guid userId, Guid chapterId, UpdateChapterRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.Story.PassportId);

        chapter.Title = request.Title;
        chapter.CategoryId = request.CategoryId;
        chapter.PlaceId = request.PlaceId;
        chapter.EventDate = request.EventDate;
        chapter.DisplayOrder = request.DisplayOrder;

        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(chapter);
    }

    private static ChapterDetailDto MapToDetail(Chapter chapter) => new()
    {
        Id = chapter.Id,
        StoryId = chapter.StoryId,
        Title = chapter.Title,
        CategoryId = chapter.CategoryId,
        PlaceId = chapter.PlaceId,
        CoverMediaId = chapter.CoverMediaId,
        EventDate = chapter.EventDate,
        DisplayOrder = chapter.DisplayOrder,
        Media = chapter.Media.Select(m => new MediaDto
        {
            Id = m.Id,
            ChapterId = m.ChapterId,
            Url = m.Url,
            Type = m.Type,
            UploadedBy = m.UploadedBy
        }).ToList()
    };
}
