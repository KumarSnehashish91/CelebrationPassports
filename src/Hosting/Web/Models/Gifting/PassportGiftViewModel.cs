namespace CelebrationPassports.Web.Models.Gifting;

public class PassportGiftViewModel
{
    public Guid Id { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }

    public string? GiftMessage { get; set; }

    public decimal Amount { get; set; }

    public string RedemptionCode { get; set; } = string.Empty;

    // "AwaitingClaim" | "Claimed"
    public string Status { get; set; } = string.Empty;

    public DateTime PurchasedOn { get; set; }

    public DateTime? ClaimedOn { get; set; }

    // "Immediate" | "Scheduled"
    public string DeliveryMode { get; set; } = "Immediate";

    public DateTime? ScheduledDeliveryDate { get; set; }

    // Filled in by the controller (needs Request.Scheme/Host, not available to the DTO).
    public string ClaimUrl { get; set; } = string.Empty;
}
