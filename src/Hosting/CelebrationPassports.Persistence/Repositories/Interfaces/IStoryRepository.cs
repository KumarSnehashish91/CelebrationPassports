using CelebrationPassports.Persistence.Entities;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IStoryRepository : IGenericRepository<Story>
{
    Task<IReadOnlyList<Story>> GetByPassportAsync(Guid passportId);

    Task<Story?> GetByIdWithChaptersAsync(Guid id);
}
