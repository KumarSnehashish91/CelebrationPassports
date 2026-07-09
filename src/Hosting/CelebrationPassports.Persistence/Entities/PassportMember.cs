using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class PassportMember
{
    public Guid PassportId { get; set; }

    public Guid UserId { get; set; }

    public int PassportMemberRoleId { get; set; }

    public DateTime JoinedOn { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual PassportMemberRole PassportMemberRole { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
