using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.MemoryMap;

namespace CelebrationPassports.Web.Services;

public class MemoryMapService : IMemoryMapService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IMediaService _mediaService;

    public MemoryMapService(HttpClient httpClient, IMediaService mediaService)
    {
        _httpClient = httpClient;
        _mediaService = mediaService;
    }

    public async Task<List<MemoryMapPinViewModel>> GetByPassportAsync(Guid passportId)
    {
        var response = await _httpClient.GetAsync($"api/passports/{passportId}/memory-map");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<PinBody>>(JsonOptions);

        if (body is null)
        {
            return [];
        }

        var result = new List<MemoryMapPinViewModel>();

        foreach (var p in body)
        {
            var imageUrl = p.CoverMediaId.HasValue
                ? await _mediaService.GetUrlAsync(p.CoverMediaId.Value)
                : null;

            result.Add(new MemoryMapPinViewModel
            {
                ChapterId = p.ChapterId,
                StoryId = p.StoryId,
                Title = p.Title,
                EventDate = p.EventDate,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                PlaceName = p.PlaceName,
                PhotoCount = p.PhotoCount,
                ImageUrl = imageUrl
            });
        }

        return result;
    }

    private sealed class PinBody
    {
        public Guid ChapterId { get; set; }
        public Guid? StoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? PlaceName { get; set; }
        public int PhotoCount { get; set; }
        public Guid? CoverMediaId { get; set; }
    }
}
