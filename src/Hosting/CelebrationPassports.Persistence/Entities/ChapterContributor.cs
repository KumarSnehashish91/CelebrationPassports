namespace CelebrationPassports.Persistence.Entities;

// The "scoped" half of Scoped Family Sharing — grants a user access to exactly one
// Chapter (view + add photos/wishes) without making them a full PassportPerson, who
// would otherwise see every chapter, the budget, and the rest of the passport.
public class ChapterContributor
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public Guid UserId { get; set; }

    public Guid InvitedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual User InvitedByUser { get; set; } = null!;
}
