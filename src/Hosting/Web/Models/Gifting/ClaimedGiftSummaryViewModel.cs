namespace CelebrationPassports.Web.Models.Gifting;

public class ClaimedGiftSummaryViewModel
{
    public string GifterName { get; set; } = string.Empty;

    public string PassportTitle { get; set; } = string.Empty;

    public int PhotoCount { get; set; }

    public bool HasMessage { get; set; }

    public Guid? ChapterId { get; set; }
}
