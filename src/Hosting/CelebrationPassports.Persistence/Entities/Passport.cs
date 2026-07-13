using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class Passport
{
    public Guid Id { get; set; }

    public Guid OwnerUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    // The Media FK is added after Media exists, to avoid the ERD's circular dependency.
    public Guid? CoverMediaId { get; set; }

    public PassportStatus Status { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual User Owner { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }
}
