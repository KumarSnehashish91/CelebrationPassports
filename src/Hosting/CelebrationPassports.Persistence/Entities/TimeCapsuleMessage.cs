namespace CelebrationPassports.Persistence.Entities;

// Feature: Time-Capsule Messages (feature-backlog-v1.1.md, RELIVE #6). Visible to the
// whole Passport (shared/family surprise), authored by one member — Content is withheld
// by the Application layer until IsUnlocked flips, not just hidden by the UI.
public class TimeCapsuleMessage
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid AuthorUserId { get; set; }

    // Short label shown even while locked (e.g. "For our 25th anniversary") — not in
    // the original spec, but a locked list with literally nothing to show isn't usable.
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime UnlockDate { get; set; }

    public bool IsUnlocked { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual User AuthorUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }
}
