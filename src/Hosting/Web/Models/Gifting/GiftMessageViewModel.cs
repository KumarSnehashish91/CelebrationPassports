namespace CelebrationPassports.Web.Models.Gifting;

public class GiftMessageViewModel
{
    // "Written" | "Voice" | "Video"
    public string Type { get; set; } = string.Empty;

    public string? Text { get; set; }

    public string? MediaUrl { get; set; }

    public string GifterName { get; set; } = string.Empty;
}
