using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Repositories.Implementations;

public class PassportInvitationRepository : GenericRepository<PassportInvitation>, IPassportInvitationRepository
{
    public PassportInvitationRepository(CelebrationPassportsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PassportInvitation>> GetPendingForEmailAsync(string email)
    {
        return await _dbcontext.PassportInvitations
            .Include(i => i.Passport)
            .Where(i => i.Email == email && i.Status == PassportInvitationStatus.Pending)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PassportInvitation>> GetByPassportAsync(Guid passportId)
    {
        return await _dbcontext.PassportInvitations
            .Include(i => i.Passport)
            .Where(i => i.PassportId == passportId)
            .ToListAsync();
    }

    public async Task<bool> ExistsPendingAsync(Guid passportId, string email)
    {
        return await _dbcontext.PassportInvitations
            .AnyAsync(i => i.PassportId == passportId && i.Email == email && i.Status == PassportInvitationStatus.Pending);
    }
}
