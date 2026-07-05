using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("Email", Name = "IX_PassportInvitations_Email")]
public partial class PassportInvitation
{
    [Key]
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    [StringLength(320)]
    public string Email { get; set; } = null!;

    public int PassportMemberRoleId { get; set; }

    public Guid InvitationToken { get; set; }

    public DateTime ExpiresOn { get; set; }

    public DateTime? AcceptedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    [ForeignKey("PassportId")]
    [InverseProperty("PassportInvitations")]
    public virtual Passport Passport { get; set; } = null!;

    [ForeignKey("PassportMemberRoleId")]
    [InverseProperty("PassportInvitations")]
    public virtual PassportMemberRole PassportMemberRole { get; set; } = null!;
}
