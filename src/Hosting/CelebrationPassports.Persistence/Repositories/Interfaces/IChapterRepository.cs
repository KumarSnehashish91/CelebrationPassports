using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IChapterRepository : IGenericRepository<Chapter>
{
    Task<Chapter?> GetByIdWithMediaAsync(Guid id);

    Task<IReadOnlyList<Chapter>> GetByStoryAsync(Guid storyId);

    Task<IReadOnlyList<Chapter>> GetByPassportsAsync(IEnumerable<Guid> passportIds, ChapterStatus? status, int? take);
}
