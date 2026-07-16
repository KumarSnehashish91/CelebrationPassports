using System.Text.Json.Serialization;

namespace CelebrationPassports.Infrastructure.AI.Models;

public class AIRequest
{
    public string Model { get; set; } = string.Empty;

    public string Prompt { get; set; } = string.Empty;

    public bool Stream { get; set; }

    // Base64-encoded image bytes — Ollama's /api/generate accepts this for
    // vision-capable models (e.g. gemma3). Omitted entirely (not just null) for plain
    // text prompts, rather than relying on Ollama tolerating a null field.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Images { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AIRequestOptions? Options { get; set; }

    // How long Ollama should keep this model loaded after the call — see
    // AIOptions.KeepAlive. Without this, Ollama's own default unloads the model a few
    // minutes after each request, so the next call pays a full reload-from-disk cost
    // before it can even start generating.
    [JsonPropertyName("keep_alive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? KeepAlive { get; set; }
}

public class AIRequestOptions
{
    // Hard cap on generated tokens — Ollama stops as soon as it's hit. The single
    // biggest lever on response time for short outputs (a photo insight, a title):
    // without it, a small local model can ramble well past what was asked for, and
    // every extra token costs real wall-clock time on CPU-only inference.
    [JsonPropertyName("num_predict")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NumPredict { get; set; }

    // Higher = more varied/specific phrasing, lower = safer/more repetitive. Ollama's
    // own default is 0.8; left null to inherit that unless a caller has a reason to
    // override it.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; set; }
}