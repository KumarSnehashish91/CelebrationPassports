using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class Passport
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int StatusId { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public Guid? CoverMediaId { get; set; }

    public virtual MomentMedium? CoverMedia { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<PassportInvitation> PassportInvitations { get; set; } = new List<PassportInvitation>();

    public virtual ICollection<PassportMember> PassportMembers { get; set; } = new List<PassportMember>();

    public virtual ICollection<PassportMoment> PassportMoments { get; set; } = new List<PassportMoment>();

    public virtual Status Status { get; set; } = null!;
}
