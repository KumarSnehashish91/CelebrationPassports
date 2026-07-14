using CelebrationPassports.Persistence.Entities;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IPassportRepository : IGenericRepository<Passport>
{
    Task<IReadOnlyList<Passport>> GetForUserAsync(Guid userId);

    Task<Passport?> GetByIdWithPeopleAsync(Guid id);

    Task<bool> IsMemberAsync(Guid passportId, Guid userId);
}
