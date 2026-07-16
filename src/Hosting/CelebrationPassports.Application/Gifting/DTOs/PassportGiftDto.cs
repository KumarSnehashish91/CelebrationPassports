namespace CelebrationPassports.Application.Gifting.DTOs;

public class PassportGiftDto
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string RecipientName { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }

    public string? GiftMessage { get; set; }

    public decimal Amount { get; set; }

    public string RedemptionCode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime PurchasedOn { get; set; }

    public DateTime? ClaimedOn { get; set; }

    // "Immediate" | "Scheduled"
    public string DeliveryMode { get; set; } = "Immediate";

    public DateTime? ScheduledDeliveryDate { get; set; }
}
