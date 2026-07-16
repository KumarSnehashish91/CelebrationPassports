using CelebrationPassports.Application.Stories.DTOs;

namespace CelebrationPassports.Application.Stories.Interfaces;

public interface IChapterService
{
    Task<ChapterDetailDto> GetByIdAsync(Guid userId, Guid chapterId);

    Task<ChapterDetailDto> CreateAsync(Guid userId, Guid storyId, CreateChapterRequest request);

    Task<ChapterDetailDto> UpdateAsync(Guid userId, Guid chapterId, UpdateChapterRequest request);

    Task<ChapterDetailDto> SetSoundtrackAsync(Guid userId, Guid chapterId, SetChapterSoundtrackRequest request);

    // Draft = auto-detected, pending review — across all of the user's passports.
    Task<IReadOnlyList<ChapterDetailDto>> ListDraftsForUserAsync(Guid userId);

    // Confirmed chapters only, most recent first — for the Dashboard's Recent Chapters widget.
    Task<IReadOnlyList<ChapterDetailDto>> ListRecentConfirmedForUserAsync(Guid userId, int take);

    Task<ChapterDetailDto> ConfirmAsync(Guid userId, Guid chapterId, ConfirmChapterRequest request);

    Task DiscardAsync(Guid userId, Guid chapterId);

    Task<IReadOnlyList<MemoryMapPinDto>> GetMemoryMapAsync(Guid userId, Guid passportId);
}
