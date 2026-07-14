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
