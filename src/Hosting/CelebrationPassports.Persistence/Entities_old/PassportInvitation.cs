using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class PassportInvitation
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Email { get; set; } = null!;

    public int PassportMemberRoleId { get; set; }

    public Guid InvitationToken { get; set; }

    public DateTime ExpiresOn { get; set; }

    public DateTime? AcceptedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual PassportMemberRole PassportMemberRole { get; set; } = null!;
}
