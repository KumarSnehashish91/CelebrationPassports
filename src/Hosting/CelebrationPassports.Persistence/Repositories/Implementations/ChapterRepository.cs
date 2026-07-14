using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Enums;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Repositories.Implementations;

public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
{
    public ChapterRepository(CelebrationPassportsDbContext context) : base(context)
    {
    }

    public async Task<Chapter?> GetByIdWithMediaAsync(Guid id)
    {
        return await _dbcontext.Chapters
            .Include(c => c.Story)
            .Include(c => c.Media.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<IReadOnlyList<Chapter>> GetByStoryAsync(Guid storyId)
    {
        return await _dbcontext.Chapters
            .AsNoTracking()
            .Where(c => c.StoryId == storyId && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Chapter>> GetByPassportsAsync(IEnumerable<Guid> passportIds, ChapterStatus? status, int? take)
    {
        var query = _dbcontext.Chapters
            .AsNoTracking()
            .Where(c => !c.IsDeleted && passportIds.Contains(c.PassportId));

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        query = query.OrderByDescending(c => c.EventDate);

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync();
    }
}
