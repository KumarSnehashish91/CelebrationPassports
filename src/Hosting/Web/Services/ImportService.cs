using System.Text.Json;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Imports;

namespace CelebrationPassports.Web.Services;

public class ImportService : IImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public ImportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ImportJobViewModel?> StartGooglePhotosImportAsync(Guid passportId, IFormFile archive)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = archive.OpenReadStream();
        using var streamContent = new StreamContent(stream);
        content.Add(streamContent, "archive", archive.FileName);

        var response = await _httpClient.PostAsync($"api/imports/passports/{passportId}/google-photos", content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ImportJobViewModel>(JsonOptions);
    }

    public async Task<ImportJobViewModel?> GetStatusAsync(Guid jobId)
    {
        var response = await _httpClient.GetAsync($"api/imports/{jobId}");

        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ImportJobViewModel>(JsonOptions)
            : null;
    }
}
