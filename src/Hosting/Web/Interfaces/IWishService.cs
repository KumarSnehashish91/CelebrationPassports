using CelebrationPassports.Web.Models.Stories;

namespace CelebrationPassports.Web.Interfaces;

public interface IWishService
{
    Task<List<WishViewModel>> GetByChapterAsync(Guid chapterId);

    Task<bool> CreateAsync(Guid chapterId, string text);

    Task<bool> DeleteAsync(Guid id);
}
