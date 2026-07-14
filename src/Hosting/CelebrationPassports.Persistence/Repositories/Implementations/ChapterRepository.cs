using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
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
}
