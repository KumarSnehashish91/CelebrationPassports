namespace CelebrationPassports.Persistence.Entities;

public class Comment
{
    public Guid Id { get; set; }

    // Exactly one of ChapterId / MediaId is set (enforced by DB check constraint).
    public Guid? ChapterId { get; set; }

    public Guid? MediaId { get; set; }

    public Guid UserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual Media? Media { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}
