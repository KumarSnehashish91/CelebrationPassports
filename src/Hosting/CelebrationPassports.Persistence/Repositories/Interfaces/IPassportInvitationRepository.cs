using CelebrationPassports.Persistence.Entities;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IPassportInvitationRepository : IGenericRepository<PassportInvitation>
{
    Task<IReadOnlyList<PassportInvitation>> GetPendingForEmailAsync(string email);

    Task<IReadOnlyList<PassportInvitation>> GetByPassportAsync(Guid passportId);

    Task<bool> ExistsPendingAsync(Guid passportId, string email);
}
