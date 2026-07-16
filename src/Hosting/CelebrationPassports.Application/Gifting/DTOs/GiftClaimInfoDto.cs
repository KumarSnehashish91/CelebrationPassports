namespace CelebrationPassports.Application.Gifting.DTOs;

// Public — shown to whoever opens the claim link, before they've necessarily logged in.
// Deliberately excludes the actual message content (written text, voice/video URL) —
// gift-message-schedule-claim.md requires the reveal to happen only after claim+login.
public class GiftClaimInfoDto
{
    public string RecipientName { get; set; } = string.Empty;

    public string GifterName { get; set; } = string.Empty;

    public string PassportTitle { get; set; } = string.Empty;

    public bool HasMessage { get; set; }

    // "a voice message" | "a video message" | "a written note" | null
    public string? MessageTypeLabel { get; set; }

    public string? CoverPhotoUrl { get; set; }

    public string? PullQuoteText { get; set; }

    public bool AlreadyClaimed { get; set; }

    public bool IsScheduledAndNotYetDue { get; set; }

    public DateTime? ScheduledDeliveryDate { get; set; }
}
