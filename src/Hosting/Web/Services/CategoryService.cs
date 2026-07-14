using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Categories;

namespace CelebrationPassports.Web.Services;

public class CategoryService : ICategoryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CategoryOptionViewModel>> ListAllAsync()
    {
        var response = await _httpClient.GetAsync("api/categories");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<CategoryOptionViewModel>>(JsonOptions);
        return body ?? [];
    }
}
