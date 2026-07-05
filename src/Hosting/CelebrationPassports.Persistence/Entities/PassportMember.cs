using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[PrimaryKey("PassportId", "UserId")]
[Index("UserId", Name = "IX_PassportMembers_UserId")]
public partial class PassportMember
{
    [Key]
    public Guid PassportId { get; set; }

    [Key]
    public Guid UserId { get; set; }

    public int PassportMemberRoleId { get; set; }

    public DateTime JoinedOn { get; set; }

    [ForeignKey("PassportId")]
    [InverseProperty("PassportMembers")]
    public virtual Passport Passport { get; set; } = null!;

    [ForeignKey("PassportMemberRoleId")]
    [InverseProperty("PassportMembers")]
    public virtual PassportMemberRole PassportMemberRole { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("PassportMembers")]
    public virtual User User { get; set; } = null!;
}
