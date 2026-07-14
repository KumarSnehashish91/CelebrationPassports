using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class Reaction
{
    public Guid Id { get; set; }

    // Exactly one of ChapterId / MediaId / CommentId is set (enforced by DB check constraint).
    public Guid? ChapterId { get; set; }

    public Guid? MediaId { get; set; }

    public Guid? CommentId { get; set; }

    public Guid UserId { get; set; }

    public ReactionType Type { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual Media? Media { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual User User { get; set; } = null!;
}
