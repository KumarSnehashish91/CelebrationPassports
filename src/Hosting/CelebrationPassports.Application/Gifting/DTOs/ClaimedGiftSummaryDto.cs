namespace CelebrationPassports.Application.Gifting.DTOs;

// Post-claim, first-run Dashboard banner (gift-message-schedule-claim.md).
public class ClaimedGiftSummaryDto
{
    public string GifterName { get; set; } = string.Empty;

    public string PassportTitle { get; set; } = string.Empty;

    public int PhotoCount { get; set; }

    public bool HasMessage { get; set; }

    // The pre-populated Chapter created at claim time, if this gift came with a
    // photo story — null for a simple (no-story) gift. Links "Read the full story".
    public Guid? ChapterId { get; set; }
}
