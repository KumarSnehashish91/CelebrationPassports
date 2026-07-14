using CelebrationPassports.Persistence.Entities;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IChapterRepository : IGenericRepository<Chapter>
{
    Task<Chapter?> GetByIdWithMediaAsync(Guid id);

    Task<IReadOnlyList<Chapter>> GetByStoryAsync(Guid storyId);
}
