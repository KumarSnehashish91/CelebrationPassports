namespace CelebrationPassports.Application.Gifting.DTOs;

// Post-claim only — see IPassportGiftService.GetMessageAsync. Never served to the
// public claim page.
public class GiftMessageDto
{
    // "Written" | "Voice" | "Video"
    public string Type { get; set; } = string.Empty;

    public string? Text { get; set; }

    public string? MediaUrl { get; set; }

    public string GifterName { get; set; } = string.Empty;
}
