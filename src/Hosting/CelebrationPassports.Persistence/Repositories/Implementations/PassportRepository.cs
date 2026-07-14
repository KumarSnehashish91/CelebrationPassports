using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Repositories.Implementations;

public class PassportRepository : GenericRepository<Passport>, IPassportRepository
{
    public PassportRepository(CelebrationPassportsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Passport>> GetForUserAsync(Guid userId)
    {
        return await _dbcontext.Passports
            .AsNoTracking()
            .Include(p => p.People.Where(person => !person.IsDeleted))
            .Where(p => !p.IsDeleted && (p.OwnerUserId == userId
                || p.People.Any(person => person.UserId == userId && !person.IsDeleted)))
            .ToListAsync();
    }

    public async Task<Passport?> GetByIdWithPeopleAsync(Guid id)
    {
        return await _dbcontext.Passports
            .Include(p => p.People.Where(person => !person.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<bool> IsMemberAsync(Guid passportId, Guid userId)
    {
        return await _dbcontext.Passports
            .AnyAsync(p => p.Id == passportId && !p.IsDeleted
                && (p.OwnerUserId == userId
                    || p.People.Any(person => person.UserId == userId && !person.IsDeleted)));
    }
}
