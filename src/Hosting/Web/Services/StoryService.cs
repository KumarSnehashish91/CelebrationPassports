using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Places;
using CelebrationPassports.Web.Models.Stories;

namespace CelebrationPassports.Web.Services;

public class StoryService : IStoryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IPassportService _passportService;
    private readonly IPlaceService _placeService;
    private readonly IMediaService _mediaService;
    private readonly ICategoryService _categoryService;

    public StoryService(
        HttpClient httpClient,
        IPassportService passportService,
        IPlaceService placeService,
        IMediaService mediaService,
        ICategoryService categoryService)
    {
        _httpClient = httpClient;
        _passportService = passportService;
        _placeService = placeService;
        _mediaService = mediaService;
        _categoryService = categoryService;
    }

    public async Task<List<StoryListItemViewModel>> GetMineAsync()
    {
        var passports = await _passportService.GetMineAsync();
        var result = new List<StoryListItemViewModel>();

        foreach (var passport in passports)
        {
            var response = await _httpClient.GetAsync($"api/passports/{passport.Id}/stories");

            if (!response.IsSuccessStatusCode)
            {
                continue;
            }

            var body = await response.Content.ReadFromJsonAsync<List<StorySummaryBody>>(JsonOptions);

            if (body is null)
            {
                continue;
            }

            foreach (var s in body)
            {
                result.Add(new StoryListItemViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    ChapterCount = s.ChapterCount
                });
            }
        }

        return result.OrderByDescending(s => s.StartDate).ToList();
    }

    public async Task<StoryDetailViewModel?> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/stories/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<StoryDetailBody>(JsonOptions);

        if (body is null)
        {
            return null;
        }

        var categories = await _categoryService.ListAllAsync();
        var categoryLookup = categories.ToDictionary(c => c.Id, c => c.Name);

        var model = new StoryDetailViewModel
        {
            Id = body.Id,
            PassportId = body.PassportId,
            Title = body.Title,
            StartDate = body.StartDate,
            EndDate = body.EndDate,
            Chapters = body.Chapters.Select(c => new ChapterSummaryViewModel
            {
                Id = c.Id,
                Title = c.Title,
                CategoryId = c.CategoryId,
                CategoryName = categoryLookup.GetValueOrDefault(c.CategoryId, "Uncategorized"),
                EventDate = c.EventDate
            }).OrderBy(c => c.EventDate).ToList()
        };

        if (body.PlaceId.HasValue)
        {
            var place = await _placeService.GetByIdAsync(body.PlaceId.Value);
            model.PlaceName = place is null ? null : place.Name + (!string.IsNullOrWhiteSpace(place.City) ? $", {place.City}" : "");
        }

        if (body.CoverMediaId.HasValue)
        {
            model.CoverImageUrl = await _mediaService.GetUrlAsync(body.CoverMediaId.Value);
        }

        return model;
    }

    public async Task<Guid?> CreateAsync(CreateStoryViewModel model)
    {
        Guid? placeId = null;

        if (!string.IsNullOrWhiteSpace(model.PlaceName))
        {
            placeId = await _placeService.CreateAsync(new CreatePlaceViewModel
            {
                Name = model.PlaceName,
                City = model.City ?? string.Empty,
                Country = "India"
            });
        }

        var request = new
        {
            title = model.Title,
            placeId,
            startDate = model.StartDate,
            endDate = model.EndDate
        };

        var response = await _httpClient.PostAsJsonAsync($"api/passports/{model.PassportId}/stories", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<StoryDetailBody>(JsonOptions);
        return body?.Id;
    }

    public async Task<Guid?> AddChapterAsync(CreateChapterViewModel model)
    {
        var request = new
        {
            title = model.Title,
            categoryId = model.CategoryId,
            eventDate = model.EventDate
        };

        var response = await _httpClient.PostAsJsonAsync($"api/stories/{model.StoryId}/chapters", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<ChapterDetailBody>(JsonOptions);
        return body?.Id;
    }

    public async Task<ChapterDetailViewModel?> GetChapterByIdAsync(Guid chapterId)
    {
        var response = await _httpClient.GetAsync($"api/chapters/{chapterId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<ChapterDetailBody>(JsonOptions);

        if (body is null)
        {
            return null;
        }

        var categories = await _categoryService.ListAllAsync();
        var categoryName = categories.FirstOrDefault(c => c.Id == body.CategoryId)?.Name ?? "Uncategorized";

        var storyResponse = await _httpClient.GetAsync($"api/stories/{body.StoryId}");
        var storyTitle = string.Empty;

        if (storyResponse.IsSuccessStatusCode)
        {
            var story = await storyResponse.Content.ReadFromJsonAsync<StoryDetailBody>(JsonOptions);
            storyTitle = story?.Title ?? string.Empty;
        }

        var model = new ChapterDetailViewModel
        {
            Id = body.Id,
            StoryId = body.StoryId,
            StoryTitle = storyTitle,
            Title = body.Title,
            CategoryId = body.CategoryId,
            CategoryName = categoryName,
            EventDate = body.EventDate
        };

        foreach (var m in body.Media)
        {
            var url = await _mediaService.GetUrlAsync(m.Id);

            model.Media.Add(new MediaItemViewModel
            {
                Id = m.Id,
                Url = url ?? string.Empty,
                Type = m.Type
            });
        }

        return model;
    }

    private sealed class StorySummaryBody
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid? PlaceId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int ChapterCount { get; set; }
    }

    private sealed class StoryDetailBody
    {
        public Guid Id { get; set; }
        public Guid PassportId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid? PlaceId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Guid? CoverMediaId { get; set; }
        public List<ChapterSummaryBody> Chapters { get; set; } = [];
    }

    private sealed class ChapterSummaryBody
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid? PlaceId { get; set; }
        public Guid? CoverMediaId { get; set; }
        public DateOnly EventDate { get; set; }
    }

    private sealed class ChapterDetailBody
    {
        public Guid Id { get; set; }
        public Guid StoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid? PlaceId { get; set; }
        public Guid? CoverMediaId { get; set; }
        public DateOnly EventDate { get; set; }
        public List<MediaBody> Media { get; set; } = [];
    }

    private sealed class MediaBody
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Type { get; set; }
    }
}
