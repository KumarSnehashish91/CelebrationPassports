using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Media.DTOs;
using CelebrationPassports.Application.Passports.Interfaces;
using CelebrationPassports.Application.Stories.DTOs;
using CelebrationPassports.Application.Stories.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Stories.Services;

public class ChapterService : IChapterService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IStoryRepository _storyRepository;
    private readonly IPassportRepository _passportRepository;
    private readonly IPassportAccessGuard _accessGuard;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateChapterRequest> _createValidator;
    private readonly IValidator<UpdateChapterRequest> _updateValidator;
    private readonly IValidator<SetChapterSoundtrackRequest> _soundtrackValidator;

    public ChapterService(
        IChapterRepository chapterRepository,
        IStoryRepository storyRepository,
        IPassportRepository passportRepository,
        IPassportAccessGuard accessGuard,
        IUnitOfWork unitOfWork,
        IValidator<CreateChapterRequest> createValidator,
        IValidator<UpdateChapterRequest> updateValidator,
        IValidator<SetChapterSoundtrackRequest> soundtrackValidator)
    {
        _chapterRepository = chapterRepository;
        _storyRepository = storyRepository;
        _passportRepository = passportRepository;
        _accessGuard = accessGuard;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _soundtrackValidator = soundtrackValidator;
    }

    public async Task<ChapterDetailDto> GetByIdAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureChapterAccessAsync(userId, chapterId);

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
            PassportId = story.PassportId,
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

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        chapter.Title = request.Title;
        chapter.CategoryId = request.CategoryId;
        chapter.PlaceId = request.PlaceId;
        chapter.EventDate = request.EventDate;
        chapter.DisplayOrder = request.DisplayOrder;

        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(chapter);
    }

    public async Task<ChapterDetailDto> SetSoundtrackAsync(Guid userId, Guid chapterId, SetChapterSoundtrackRequest request)
    {
        await _soundtrackValidator.ValidateAndThrowAsync(request);

        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        chapter.SongTitle = string.IsNullOrWhiteSpace(request.SongTitle) ? null : request.SongTitle.Trim();
        chapter.SongArtist = string.IsNullOrWhiteSpace(request.SongArtist) ? null : request.SongArtist.Trim();
        chapter.SongLinkUrl = string.IsNullOrWhiteSpace(request.SongLinkUrl) ? null : request.SongLinkUrl.Trim();

        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(chapter);
    }

    public async Task<IReadOnlyList<ChapterDetailDto>> ListDraftsForUserAsync(Guid userId)
    {
        var passportIds = (await _passportRepository.GetForUserAsync(userId)).Select(p => p.Id).ToList();

        if (passportIds.Count == 0)
        {
            return [];
        }

        var chapters = await _chapterRepository.GetByPassportsAsync(passportIds, ChapterStatus.Draft, take: null);

        return chapters.Select(MapToDetail).ToList();
    }

    public async Task<IReadOnlyList<ChapterDetailDto>> ListRecentConfirmedForUserAsync(Guid userId, int take)
    {
        var passportIds = (await _passportRepository.GetForUserAsync(userId)).Select(p => p.Id).ToList();

        if (passportIds.Count == 0)
        {
            return [];
        }

        var chapters = await _chapterRepository.GetByPassportsAsync(passportIds, ChapterStatus.Confirmed, take);

        return chapters.Select(MapToDetail).ToList();
    }

    public async Task<ChapterDetailDto> ConfirmAsync(Guid userId, Guid chapterId, ConfirmChapterRequest request)
    {
        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        if (chapter.Status != ChapterStatus.Draft)
        {
            throw new ConflictException("This chapter has already been confirmed.");
        }

        chapter.Title = request.Title;
        chapter.CategoryId = request.CategoryId;
        chapter.PlaceId = request.PlaceId;
        chapter.EventDate = request.EventDate;

        if (request.ExistingStoryId.HasValue)
        {
            var existingStory = await _storyRepository.GetByIdAsync(request.ExistingStoryId.Value)
                ?? throw new NotFoundException("Story not found.");

            if (existingStory.PassportId != chapter.PassportId)
            {
                throw new ForbiddenAccessException("That story doesn't belong to the same passport as this chapter.");
            }

            chapter.StoryId = existingStory.Id;
        }
        else
        {
            // AI-suggested title placeholder — no real title-generation model wired up
            // yet, so this is a deterministic stand-in the user can freely rename (here
            // or anytime after, from the Story itself).
            var suggestedTitle = string.IsNullOrWhiteSpace(request.NewStoryTitle)
                ? $"Trip - {request.EventDate:MMMM yyyy}"
                : request.NewStoryTitle;

            var newStory = new Story
            {
                Id = Guid.NewGuid(),
                PassportId = chapter.PassportId,
                Title = suggestedTitle,
                PlaceId = request.PlaceId,
                StartDate = request.EventDate,
                EndDate = request.EventDate,
                DisplayOrder = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _storyRepository.AddAsync(newStory);
            chapter.StoryId = newStory.Id;
        }

        chapter.Status = ChapterStatus.Confirmed;

        await _unitOfWork.SaveChangesAsync();

        return MapToDetail(chapter);
    }

    public async Task DiscardAsync(Guid userId, Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdWithMediaAsync(chapterId)
            ?? throw new NotFoundException("Chapter not found.");

        await _accessGuard.EnsureMemberAsync(userId, chapter.PassportId);

        if (chapter.Status != ChapterStatus.Draft)
        {
            throw new ConflictException("Only a pending (unconfirmed) chapter can be discarded.");
        }

        // Unattach rather than delete the photos — "upload first, sort later" still
        // holds for a discarded detection; nothing the user actually uploaded is lost.
        foreach (var media in chapter.Media)
        {
            media.ChapterId = null;
        }

        chapter.IsDeleted = true;
        chapter.DeletedOn = DateTime.UtcNow;
        chapter.DeletedBy = userId;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<MemoryMapPinDto>> GetMemoryMapAsync(Guid userId, Guid passportId)
    {
        await _accessGuard.EnsureMemberAsync(userId, passportId);

        var chapters = await _chapterRepository.GetMemoryMapCandidatesAsync(passportId);

        return chapters.Select(c => new MemoryMapPinDto
        {
            ChapterId = c.Id,
            StoryId = c.StoryId,
            Title = c.Title,
            EventDate = c.EventDate,
            Latitude = c.Place!.Latitude!.Value,
            Longitude = c.Place!.Longitude!.Value,
            PlaceName = c.Place!.Name,
            PhotoCount = c.Media.Count,
            CoverMediaId = c.CoverMediaId
        }).ToList();
    }

    private static ChapterDetailDto MapToDetail(Chapter chapter) => new()
    {
        Id = chapter.Id,
        PassportId = chapter.PassportId,
        StoryId = chapter.StoryId,
        Title = chapter.Title,
        CategoryId = chapter.CategoryId,
        PlaceId = chapter.PlaceId,
        CoverMediaId = chapter.CoverMediaId,
        EventDate = chapter.EventDate,
        DisplayOrder = chapter.DisplayOrder,
        Status = chapter.Status,
        Source = chapter.Source,
        SongTitle = chapter.SongTitle,
        SongArtist = chapter.SongArtist,
        SongLinkUrl = chapter.SongLinkUrl,
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
