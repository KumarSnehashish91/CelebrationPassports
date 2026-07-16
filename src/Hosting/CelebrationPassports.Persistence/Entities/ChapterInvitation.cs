using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

// Scoped Family Sharing (feature-backlog-v1.1.md, CELEBRATE #10) — a chapter-scoped
// counterpart to PassportInvitation. Accepting one creates a ChapterContributor rather
// than a full PassportPerson, so the invited guest only ever gets access to this one
// chapter, not the whole passport.
public class ChapterInvitation
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public Guid InvitedBy { get; set; }

    public string Email { get; set; } = string.Empty;

    public ChapterInvitationStatus Status { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;

    public virtual User InvitedByUser { get; set; } = null!;
}
