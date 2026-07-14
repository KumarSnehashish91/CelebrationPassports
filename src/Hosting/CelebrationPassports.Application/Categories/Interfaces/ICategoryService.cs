using CelebrationPassports.Application.Categories.DTOs;

namespace CelebrationPassports.Application.Categories.Interfaces;

public interface ICategoryService
{
    // No passport-scoping — Categories are a shared, fixed lookup list (same as
    // EventType), not owned data. Seeded once via migration, not user-creatable today.
    Task<IReadOnlyList<CategoryDto>> ListAllAsync();
}
