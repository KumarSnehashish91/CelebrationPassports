using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

// Feature: Import Existing Memories (feature-backlog-v1.1.md, PRESERVE #1). v1 supports
// a Google Photos Takeout export (.zip of media + JSON sidecars) only — WhatsApp export
// parsing is explicitly flagged in the backlog as fragile enough to scope separately.
// Imported media lands unassigned (ChapterId = null, PendingClustering = true) and is
// picked up by the existing AutoChapterClusteringBackgroundService exactly like a
// QuickUpload batch — no import-specific clustering logic. PassportId is carried here for
// access-control and status display; it does NOT scope where the clustering sweep places
// the resulting chapters (Media has no PassportId — see IAutoChapterClusteringService),
// so a user with more than one Passport inherits that same pre-existing ambiguity.
public class ImportJob
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public ImportSourceType SourceType { get; set; }

    public ImportJobStatus Status { get; set; } = ImportJobStatus.Pending;

    // Absolute filesystem path to the staged archive (see IImportArchiveStorage) — not a
    // web-servable URL, unlike Media.Url.
    public string ArchivePath { get; set; } = string.Empty;

    public int TotalItems { get; set; }

    public int ProcessedItems { get; set; }

    public int SkippedItems { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual User CreatedByUser { get; set; } = null!;
}
