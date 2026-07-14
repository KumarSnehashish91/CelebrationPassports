using CelebrationPassports.Web.Models.Categories;

namespace CelebrationPassports.Web.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryOptionViewModel>> ListAllAsync();
}
