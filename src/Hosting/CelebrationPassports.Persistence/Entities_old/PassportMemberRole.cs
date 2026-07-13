using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class PassportMemberRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public short DisplayOrder { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<PassportInvitation> PassportInvitations { get; set; } = new List<PassportInvitation>();

    public virtual ICollection<PassportMember> PassportMembers { get; set; } = new List<PassportMember>();
}
