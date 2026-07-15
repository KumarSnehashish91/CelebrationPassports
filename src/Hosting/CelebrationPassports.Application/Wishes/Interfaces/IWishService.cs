using CelebrationPassports.Application.Wishes.DTOs;

namespace CelebrationPassports.Application.Wishes.Interfaces;

public interface IWishService
{
    Task<WishDto> CreateAsync(Guid userId, Guid chapterId, CreateWishRequest request);

    Task<IReadOnlyList<WishDto>> GetByChapterAsync(Guid userId, Guid chapterId);

    Task DeleteAsync(Guid userId, Guid wishId);
}
