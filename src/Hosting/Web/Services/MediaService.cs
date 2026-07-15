using System.Text.Json;
using CelebrationPassports.Web.Interfaces;

namespace CelebrationPassports.Web.Services;

public class MediaService : IMediaService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public MediaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Guid?> UploadAsync(IFormFile file, bool pendingClustering = false)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var streamContent = new StreamContent(stream);
        content.Add(streamContent, "file", file.FileName);

        var url = pendingClustering ? "api/media?pendingClustering=true" : "api/media";
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<MediaBody>(JsonOptions);
        return body?.Id;
    }

    public async Task<Guid?> UploadToChapterAsync(Guid chapterId, IFormFile file)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var streamContent = new StreamContent(stream);
        content.Add(streamContent, "file", file.FileName);

        var response = await _httpClient.PostAsync($"api/media/chapters/{chapterId}", content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<MediaBody>(JsonOptions);
        return body?.Id;
    }

    public async Task<string?> GetUrlAsync(Guid mediaId)
    {
        var response = await _httpClient.GetAsync($"api/media/{mediaId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadFromJsonAsync<MediaBody>(JsonOptions);

        if (body is null || string.IsNullOrEmpty(body.Url))
        {
            return null;
        }

        // Url is relative (e.g. "/uploads/x.jpg"), served by the API host — Web renders
        // on a different port, so resolve it against the API's base address to get
        // something the browser can actually load.
        return _httpClient.BaseAddress is null
            ? body.Url
            : new Uri(_httpClient.BaseAddress, body.Url).ToString();
    }

    private sealed class MediaBody
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
