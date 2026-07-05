using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[PrimaryKey("UserId", "RoleId")]
[Index("RoleId", Name = "IX_UserRoles_RoleId")]
public partial class UserRole
{
    [Key]
    public Guid UserId { get; set; }

    [Key]
    public int RoleId { get; set; }

    public DateTime AssignedOn { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserRoles")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserRoles")]
    public virtual User User { get; set; } = null!;
}
