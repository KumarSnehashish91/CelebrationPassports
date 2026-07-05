using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("UserId", Name = "IX_UserTokens_UserId")]
public partial class UserToken
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [StringLength(50)]
    public string TokenType { get; set; } = null!;

    [StringLength(500)]
    public string Token { get; set; } = null!;

    public DateTime ExpiresOn { get; set; }

    public DateTime? UsedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserTokens")]
    public virtual User User { get; set; } = null!;
}
