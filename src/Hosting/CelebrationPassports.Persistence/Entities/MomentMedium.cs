using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("PassportMomentId", Name = "IX_MomentMedia_PassportMomentId")]
public partial class MomentMedium
{
    [Key]
    public Guid Id { get; set; }

    public Guid PassportMomentId { get; set; }

    public int MediaTypeId { get; set; }

    [StringLength(500)]
    public string FileName { get; set; } = null!;

    [StringLength(1000)]
    public string FilePath { get; set; } = null!;

    public long? FileSize { get; set; }

    [StringLength(200)]
    public string? MimeType { get; set; }

    [StringLength(500)]
    public string? Caption { get; set; }

    public DateTime CreatedOn { get; set; }

    [ForeignKey("MediaTypeId")]
    [InverseProperty("MomentMedia")]
    public virtual MediaType MediaType { get; set; } = null!;

    [ForeignKey("PassportMomentId")]
    [InverseProperty("MomentMedia")]
    public virtual PassportMoment PassportMoment { get; set; } = null!;

    [InverseProperty("CoverMedia")]
    public virtual ICollection<PassportMoment> PassportMoments { get; set; } = new List<PassportMoment>();

    [InverseProperty("CoverMedia")]
    public virtual ICollection<Passport> Passports { get; set; } = new List<Passport>();
}
