using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

// A guest's submission via an unguessable per-chapter link — never visible anywhere
// until a passport member approves it (Status starts Pending; PhotoUrl is raw storage
// output, not yet a real Media row — ApprovedMedia only gets created on approval).
public class GuestbookSubmission
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string GuestName { get; set; } = string.Empty;

    public string? Message { get; set; }

    public string? PhotoUrl { get; set; }

    public GuestbookSubmissionStatus Status { get; set; } = GuestbookSubmissionStatus.Pending;

    public DateTime CreatedAt { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public Guid? ApprovedMediaId { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;

    public virtual User? ReviewedByUser { get; set; }

    public virtual Media? ApprovedMedia { get; set; }
}
