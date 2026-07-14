using System.Net.Http.Json;
using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Places;

namespace CelebrationPassports.Web.Services;

public class PlaceService : IPlaceService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public PlaceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PlaceSearchResultViewModel>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var response = await _httpClient.GetAsync($"api/places?search={Uri.EscapeDataString(query)}");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<List<PlaceBody>>(JsonOptions);

        return body?.Select(p => new PlaceSearchResultViewModel
        {
            Id = p.Id,
            Name = p.Name,
            City = p.City,
            Country = p.Country
        }).ToList() ?? [];
    }

    public async Task<Guid?> CreateAsync(CreatePlaceViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/places", new
        {
            name = model.Name,
            address = model.Address,
            city = model.City,
            postalCode = model.PostalCode,
            country = model.Country,
            notes = model.Notes
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<PlaceBody>(JsonOptions);
        return body?.Id;
    }

    public async Task<PlaceDetailViewModel?> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/places/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<PlaceBody>(JsonOptions);

        if (body is null)
        {
            return null;
        }

        return new PlaceDetailViewModel
        {
            Id = body.Id,
            Name = body.Name,
            Address = body.Address,
            City = body.City,
            PostalCode = body.PostalCode,
            Country = body.Country,
            Notes = body.Notes
        };
    }

    private sealed class PlaceBody
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Notes { get; set; }
    }
}
