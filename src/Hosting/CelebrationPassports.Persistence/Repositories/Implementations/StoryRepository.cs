using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Repositories.Implementations;

public class StoryRepository : GenericRepository<Story>, IStoryRepository
{
    public StoryRepository(CelebrationPassportsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Story>> GetByPassportAsync(Guid passportId)
    {
        return await _dbcontext.Stories
            .AsNoTracking()
            .Where(s => s.PassportId == passportId && !s.IsDeleted)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Story?> GetByIdWithChaptersAsync(Guid id)
    {
        return await _dbcontext.Stories
            .Include(s => s.Chapters.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }
}
