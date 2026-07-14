using CelebrationPassports.Web.Models.Stories;

namespace CelebrationPassports.Web.Interfaces;

public interface IStoryService
{
    // Story endpoints are passport-scoped on the API (no cross-passport "mine" endpoint
    // yet, unlike Events) — this fans out over the user's passports and merges results.
    // Fine at this app's scale (a handful of passports per user); worth a dedicated
    // endpoint later if that stops being true.
    Task<List<StoryListItemViewModel>> GetMineAsync();

    Task<StoryDetailViewModel?> GetByIdAsync(Guid id);

    Task<Guid?> CreateAsync(CreateStoryViewModel model);

    Task<Guid?> AddChapterAsync(CreateChapterViewModel model);

    Task<ChapterDetailViewModel?> GetChapterByIdAsync(Guid chapterId);
}
