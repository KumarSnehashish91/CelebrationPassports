using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("PassportId", Name = "IX_PassportMoments_PassportId")]
public partial class PassportMoment
{
    [Key]
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(2000)]
    public string? Description { get; set; }

    public DateTime MomentDate { get; set; }

    [StringLength(500)]
    public string? Location { get; set; }

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
    [InverseProperty("PassportMoments")]
    public virtual MomentMedium? CoverMedia { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("PassportMomentCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("DeletedBy")]
    [InverseProperty("PassportMomentDeletedByNavigations")]
    public virtual User? DeletedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("PassportMomentModifiedByNavigations")]
    public virtual User? ModifiedByNavigation { get; set; }

    [InverseProperty("PassportMoment")]
    public virtual ICollection<MomentMedium> MomentMedia { get; set; } = new List<MomentMedium>();

    [ForeignKey("PassportId")]
    [InverseProperty("PassportMoments")]
    public virtual Passport Passport { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("PassportMoments")]
    public virtual Status Status { get; set; } = null!;
}
