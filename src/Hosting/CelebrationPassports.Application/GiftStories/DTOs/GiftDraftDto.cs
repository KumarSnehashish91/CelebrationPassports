namespace CelebrationPassports.Application.GiftStories.DTOs;

public class GiftDraftDto
{
    public Guid Id { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }

    public string? PassportTitle { get; set; }

    public string? PersonalMessage { get; set; }

    // "DraftPhotos" | "DraftInsights" | "DraftStory" | "DraftPrint" | "Sent"
    public string Status { get; set; } = string.Empty;

    // "LifeBook" | "PassportEdition" | null
    public string? PrintFormat { get; set; }

    // "Written" | "Voice" | "Video" | null
    public string? MessageType { get; set; }

    public string? MessageMediaUrl { get; set; }

    // "Immediate" | "Scheduled"
    public string DeliveryMode { get; set; } = "Immediate";

    public DateTime? ScheduledDeliveryDate { get; set; }

    public List<GiftPhotoDto> Photos { get; set; } = [];

    public int InsightedCount => Photos.Count(p => !string.IsNullOrWhiteSpace(p.UserInsight));
}
