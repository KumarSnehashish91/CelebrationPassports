using CelebrationPassports.Application.Stories.DTOs;

namespace CelebrationPassports.Application.Stories.Interfaces;

public interface IStoryService
{
    Task<IReadOnlyList<StorySummaryDto>> ListByPassportAsync(Guid userId, Guid passportId);

    Task<StoryDetailDto> GetByIdAsync(Guid userId, Guid storyId);

    Task<StoryDetailDto> CreateAsync(Guid userId, Guid passportId, CreateStoryRequest request);
}
