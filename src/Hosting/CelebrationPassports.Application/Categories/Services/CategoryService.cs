using CelebrationPassports.Application.Categories.DTOs;
using CelebrationPassports.Application.Categories.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;

namespace CelebrationPassports.Application.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;

    public CategoryService(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> ListAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Icon = c.Icon })
            .ToList();
    }
}
