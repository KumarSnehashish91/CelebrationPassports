namespace CelebrationPassports.Web.Models.GiftStories;

public class GiftDraftViewModel
{
    public Guid Id { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }

    public string? PassportTitle { get; set; }

    public string? PersonalMessage { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? PrintFormat { get; set; }

    // "Written" | "Voice" | "Video" | null
    public string? MessageType { get; set; }

    public string? MessageMediaUrl { get; set; }

    // "Immediate" | "Scheduled"
    public string DeliveryMode { get; set; } = "Immediate";

    public DateTime? ScheduledDeliveryDate { get; set; }

    public List<GiftPhotoViewModel> Photos { get; set; } = [];

    public int InsightedCount => Photos.Count(p => !string.IsNullOrWhiteSpace(p.UserInsight));
}
