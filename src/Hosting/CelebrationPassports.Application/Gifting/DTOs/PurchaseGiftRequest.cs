namespace CelebrationPassports.Application.Gifting.DTOs;

public class PurchaseGiftRequest
{
    public string RecipientName { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }

    public string? GiftMessage { get; set; }

    // Falls back to a friendly default (e.g. "{RecipientName}'s Celebration Passport")
    // when left blank — see PassportGiftService.
    public string? PassportTitle { get; set; }
}
