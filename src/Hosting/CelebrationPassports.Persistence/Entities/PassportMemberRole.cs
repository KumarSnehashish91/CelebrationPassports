using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("Code", Name = "UQ_PassportMemberRoles_Code", IsUnique = true)]
[Index("Name", Name = "UQ_PassportMemberRoles_Name", IsUnique = true)]
public partial class PassportMemberRole
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

    [InverseProperty("PassportMemberRole")]
    public virtual ICollection<PassportInvitation> PassportInvitations { get; set; } = new List<PassportInvitation>();

    [InverseProperty("PassportMemberRole")]
    public virtual ICollection<PassportMember> PassportMembers { get; set; } = new List<PassportMember>();
}
