namespace CelebrationPassports.Web.Models.Gifting;

public class GiftClaimInfoViewModel
{
    public string RecipientName { get; set; } = string.Empty;

    public string GifterName { get; set; } = string.Empty;

    public string PassportTitle { get; set; } = string.Empty;

    public bool HasMessage { get; set; }

    public string? MessageTypeLabel { get; set; }

    public string? CoverPhotoUrl { get; set; }

    public string? PullQuoteText { get; set; }

    public bool AlreadyClaimed { get; set; }

    public bool IsScheduledAndNotYetDue { get; set; }

    public DateTime? ScheduledDeliveryDate { get; set; }
}
