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

    public virtual Media? CoverMedia { get; set; }

    public virtual ICollection<PassportPerson> People { get; set; } = new List<PassportPerson>();

    public virtual ICollection<PassportInvitation> Invitations { get; set; } = new List<PassportInvitation>();

    public virtual ICollection<PassportShare> Shares { get; set; } = new List<PassportShare>();

    public virtual ICollection<PassportOwnershipHistory> OwnershipHistory { get; set; } = new List<PassportOwnershipHistory>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Story> Stories { get; set; } = new List<Story>();

    public virtual ICollection<SomedayIdea> SomedayIdeas { get; set; } = new List<SomedayIdea>();

    public virtual ICollection<TimeCapsuleMessage> TimeCapsuleMessages { get; set; } = new List<TimeCapsuleMessage>();

    public virtual ICollection<PassportBook> PassportBooks { get; set; } = new List<PassportBook>();

    public virtual ICollection<PassportStamp> PassportStamps { get; set; } = new List<PassportStamp>();

    public virtual ICollection<PassportMilestoneProgress> MilestoneProgress { get; set; } = new List<PassportMilestoneProgress>();

    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}
