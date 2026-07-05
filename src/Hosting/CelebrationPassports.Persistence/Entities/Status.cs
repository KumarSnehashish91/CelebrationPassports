using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("Code", Name = "UQ_Statuses_Code", IsUnique = true)]
[Index("Name", Name = "UQ_Statuses_Name", IsUnique = true)]
public partial class Status
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Code { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    [InverseProperty("Status")]
    public virtual ICollection<PassportMoment> PassportMoments { get; set; } = new List<PassportMoment>();

    [InverseProperty("Status")]
    public virtual ICollection<Passport> Passports { get; set; } = new List<Passport>();

    [InverseProperty("Status")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
