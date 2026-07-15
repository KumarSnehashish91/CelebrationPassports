using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class Media
{
    public Guid Id { get; set; }

    // Null = uploaded but not yet sorted into a chapter.
    public Guid? ChapterId { get; set; }

    public Guid UploadedBy { get; set; }

    public string Url { get; set; } = string.Empty;

    public MediaType Type { get; set; }

    // Extracted from EXIF at upload time (photos only — null when absent, e.g. no GPS
    // tag, screenshots, videos). Drives trip detection: compared against the uploader's
    // UserProfile.HomePlaceId to tell whether a batch of uploads was taken away from home.
    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? CapturedAt { get; set; }

    // When this row was uploaded (not when the photo was taken — see CapturedAt for
    // that). Drives the auto-chapter clustering debounce: the background sweep only
    // considers unassigned media whose upload has "settled" for a few minutes, so a
    // rapid burst of uploads clusters once instead of repeatedly mid-upload.
    public DateTime UploadedOn { get; set; }

    // True only for uploads meant to eventually land in a Chapter via auto-clustering
    // (Stories/QuickUpload). False for unattached uploads used for something else
    // entirely (e.g. an Event's cover photo) — those also have ChapterId == null while
    // pending, but must never be swept into an auto-generated chapter.
    public bool PendingClustering { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual User UploadedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual ICollection<MediaVariant> Variants { get; set; } = new List<MediaVariant>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}
