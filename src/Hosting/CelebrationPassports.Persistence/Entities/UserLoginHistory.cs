using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Table("UserLoginHistory")]
[Index("UserId", Name = "IX_UserLoginHistory_UserId")]
public partial class UserLoginHistory
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime LoginOn { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    [StringLength(1000)]
    public string? UserAgent { get; set; }

    public bool IsSuccessful { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserLoginHistories")]
    public virtual User User { get; set; } = null!;
}
