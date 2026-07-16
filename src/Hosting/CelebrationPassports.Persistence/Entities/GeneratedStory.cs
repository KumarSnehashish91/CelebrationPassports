using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

// One row per GiftDraft — regenerating (RegenerationCount) overwrites this same row
// rather than creating a new one, so UserInsight values on GiftPhoto (the source data)
// are never at risk of being lost by a regenerate.
public class GeneratedStory
{
    public Guid Id { get; set; }

    public Guid GiftDraftId { get; set; }

    public string Title { get; set; } = string.Empty;

    // Always AI-origin — the narrative "frame" around the real photo insights.
    public string OpeningParagraph { get; set; } = string.Empty;

    public string ClosingParagraph { get; set; } = string.Empty;

    public string? PullQuoteText { get; set; }

    public ParagraphOrigin? PullQuoteOrigin { get; set; }

    // JSON array of { Text, Origin, SourcePhotoId } — the per-photo insights, in
    // display order, excluding whichever one was pulled out as the pull-quote.
    public string BodyParagraphsJson { get; set; } = "[]";

    public DateTime GeneratedAt { get; set; }

    public int RegenerationCount { get; set; }

    public virtual GiftDraft GiftDraft { get; set; } = null!;
}
