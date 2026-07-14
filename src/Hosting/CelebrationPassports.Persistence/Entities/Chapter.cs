using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class Chapter
{
    public Guid Id { get; set; }

    // The authoritative passport-access link — set directly at creation and independent
    // of StoryId, since a Draft chapter has no Story yet but still needs to be scoped to
    // a passport for access checks (previously derived via Story.PassportId, which broke
    // once StoryId became nullable).
    public Guid PassportId { get; set; }

    // Nullable — a Draft chapter (auto-detected, pending review) has no Story yet; it
    // gets one when confirmed, either an existing Story or a newly created one.
    public Guid? StoryId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? PlaceId { get; set; }

    public Guid? CoverMediaId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public int DisplayOrder { get; set; }

    public ChapterStatus Status { get; set; } = ChapterStatus.Confirmed;

    public ChapterSource Source { get; set; } = ChapterSource.Manual;

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual Story? Story { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Place? Place { get; set; }

    public virtual Media? CoverMedia { get; set; }

    public virtual User? DeletedByUser { get; set; }

    public virtual ICollection<Media> Media { get; set; } = new List<Media>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    public virtual ICollection<PassportBookChapter> BookChapters { get; set; } = new List<PassportBookChapter>();

    public virtual ICollection<PassportStamp> PassportStamps { get; set; } = new List<PassportStamp>();
}
