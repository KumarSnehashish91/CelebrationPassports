namespace CelebrationPassports.Persistence.Entities;

public class GiftPhoto
{
    public Guid Id { get; set; }

    public Guid GiftDraftId { get; set; }

    public string Url { get; set; } = string.Empty;

    // Null if the giver left it blank — AiGeneratedInsight fills in at Step 3.
    // UserInsight always takes precedence wherever both could apply.
    public string? UserInsight { get; set; }

    public string? AiGeneratedInsight { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual GiftDraft GiftDraft { get; set; } = null!;
}
