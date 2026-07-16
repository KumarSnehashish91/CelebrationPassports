using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using CelebrationPassports.Infrastructure.AI.Models;
using CelebrationPassports.Infrastructure.AI.Configuration;
using SkiaSharp;

namespace CelebrationPassports.Infrastructure.AI.Clients;

public class AIClient
{
    // A phone photo (often 3000px+ on the long edge) carries far more resolution than
    // a vision model's own internal encoder uses — sending it full-size just means more
    // bytes to transfer/decode for no quality benefit. Downscaling before the call is a
    // straightforward, real win on response time for CPU-only local inference.
    private const int MaxVisionDimension = 1024;

    private readonly HttpClient _httpClient;
    private readonly AIOptions _options;

    public AIClient(HttpClient httpClient, IOptions<AIOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    // Plain text (trip itineraries, recaps, story narratives, Gift Story's title/
    // opening/closing) never needs vision — routed at TextModel, a much smaller/faster
    // model, rather than paying the vision-capable model's cost for a job that doesn't
    // need it.
    public async Task<string> GenerateAsync(string prompt, int? maxTokens = null)
    {
        var model = string.IsNullOrWhiteSpace(_options.TextModel) ? _options.Model : _options.TextModel;
        var requestOptions = maxTokens is null ? null : new AIRequestOptions { NumPredict = maxTokens };
        return await GenerateInternalAsync(model, prompt, images: null, requestOptions);
    }

    // Vision-capable variant (see AIRequest.Images) — used by Gift Story photo insight
    // generation (gift-story-print-preview.md), the only caller that actually needs
    // image input, so this is the one path that pays for the larger vision model.
    public async Task<string> GenerateWithImageAsync(string prompt, byte[] imageBytes, int? maxTokens = null, double? temperature = null)
    {
        var resized = ResizeForVision(imageBytes);

        AIRequestOptions? requestOptions = maxTokens is null && temperature is null
            ? null
            : new AIRequestOptions { NumPredict = maxTokens, Temperature = temperature };

        return await GenerateInternalAsync(_options.Model, prompt, images: [Convert.ToBase64String(resized)], requestOptions);
    }

    private async Task<string> GenerateInternalAsync(string model, string prompt, List<string>? images, AIRequestOptions? requestOptions)
    {
        var request = new AIRequest
        {
            Model = model,
            Prompt = prompt,
            Stream = false,
            Images = images,
            Options = requestOptions,
            KeepAlive = _options.KeepAlive
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_options.BaseUrl}/api/generate",
            request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AIResponse>();

        return result?.Response ?? string.Empty;
    }

    // Never blocks the call on a resize failure (corrupt/unsupported image format,
    // decode error, etc.) — falls back to the original bytes so the vision call still
    // gets a chance to succeed, same "degrade, don't crash" approach used elsewhere for
    // AI-adjacent parsing (PhotoMetadataService, GooglePhotosImportParser).
    private static byte[] ResizeForVision(byte[] imageBytes)
    {
        try
        {
            using var original = SKBitmap.Decode(imageBytes);

            if (original is null || (original.Width <= MaxVisionDimension && original.Height <= MaxVisionDimension))
            {
                return imageBytes;
            }

            var scale = MaxVisionDimension / (double)Math.Max(original.Width, original.Height);
            var newWidth = Math.Max(1, (int)Math.Round(original.Width * scale));
            var newHeight = Math.Max(1, (int)Math.Round(original.Height * scale));

            using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium);

            if (resized is null)
            {
                return imageBytes;
            }

            using var image = SKImage.FromBitmap(resized);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);

            return data.ToArray();
        }
        catch
        {
            return imageBytes;
        }
    }
}
