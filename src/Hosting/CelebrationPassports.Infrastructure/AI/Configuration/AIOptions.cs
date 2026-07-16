namespace CelebrationPassports.Infrastructure.AI.Configuration;

public class AIOptions
{
    public string BaseUrl { get; set; } = string.Empty;

    // Vision-capable — required for Gift Story's photo insight generation, the only
    // caller that actually needs image input.
    public string Model { get; set; } = string.Empty;

    // Plain-text generation (trip itineraries, recaps, story narratives, Gift Story's
    // title/opening/closing) doesn't need vision at all — a much smaller model handles
    // it fine and is dramatically faster on this project's CPU-only local Ollama setup,
    // where there's no GPU to lean on. Falls back to Model if left unset.
    public string? TextModel { get; set; }

    // How long Ollama keeps a model resident in memory after a call (e.g. "30m"). With
    // no keep_alive, Ollama's default unloads the model a few minutes after each
    // request, so the NEXT call pays the full multi-second cost of reloading it from
    // disk before generation even starts — on top of the generation time itself.
    public string KeepAlive { get; set; } = "30m";
}