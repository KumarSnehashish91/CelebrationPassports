using System.ComponentModel.DataAnnotations;

namespace CelebrationPassports.Web.Models.Gifting;

public class PurchaseGiftViewModel
{
    [Required(ErrorMessage = "Recipient name is required.")]
    [Display(Name = "Recipient Name")]
    public string RecipientName { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Recipient Email (optional)")]
    public string? RecipientEmail { get; set; }

    [Display(Name = "Personal Message (optional)")]
    public string? GiftMessage { get; set; }

    [Display(Name = "Passport Title (optional)")]
    public string? PassportTitle { get; set; }
}
