using CelebrationPassports.Application.Stories.DTOs;

namespace CelebrationPassports.Application.Stories.Interfaces;

public interface IChapterService
{
    Task<ChapterDetailDto> GetByIdAsync(Guid userId, Guid chapterId);

    Task<ChapterDetailDto> CreateAsync(Guid userId, Guid storyId, CreateChapterRequest request);

    Task<ChapterDetailDto> UpdateAsync(Guid userId, Guid chapterId, UpdateChapterRequest request);
}
