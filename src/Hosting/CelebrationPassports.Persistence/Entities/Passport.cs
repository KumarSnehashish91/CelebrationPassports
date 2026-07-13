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

    public virtual ICollection<PassportPerson> People { get; set; } = new List<PassportPerson>();

    public virtual ICollection<PassportInvitation> Invitations { get; set; } = new List<PassportInvitation>();

    public virtual ICollection<PassportShare> Shares { get; set; } = new List<PassportShare>();

    public virtual ICollection<PassportOwnershipHistory> OwnershipHistory { get; set; } = new List<PassportOwnershipHistory>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
