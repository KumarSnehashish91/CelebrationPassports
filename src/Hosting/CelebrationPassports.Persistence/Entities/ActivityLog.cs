using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class ActivityLog
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid ActorUserId { get; set; }

    public ActivityVerb Verb { get; set; }

    public string SubjectType { get; set; } = string.Empty;

    // Intentionally not a real FK — can point at any entity type identified by SubjectType.
    public Guid SubjectId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual User ActorUser { get; set; } = null!;
}
