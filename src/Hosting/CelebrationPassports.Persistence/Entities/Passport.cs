using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("CreatedBy", Name = "IX_Passports_CreatedBy")]
[Index("StatusId", Name = "IX_Passports_StatusId")]
public partial class Passport
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
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

    [ForeignKey("CoverMediaId")]
    [InverseProperty("Passports")]
    public virtual MomentMedium? CoverMedia { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("PassportCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("DeletedBy")]
    [InverseProperty("PassportDeletedByNavigations")]
    public virtual User? DeletedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("PassportModifiedByNavigations")]
    public virtual User? ModifiedByNavigation { get; set; }

    [InverseProperty("Passport")]
    public virtual ICollection<PassportInvitation> PassportInvitations { get; set; } = new List<PassportInvitation>();

    [InverseProperty("Passport")]
    public virtual ICollection<PassportMember> PassportMembers { get; set; } = new List<PassportMember>();

    [InverseProperty("Passport")]
    public virtual ICollection<PassportMoment> PassportMoments { get; set; } = new List<PassportMoment>();

    [ForeignKey("StatusId")]
    [InverseProperty("Passports")]
    public virtual Status Status { get; set; } = null!;
}
