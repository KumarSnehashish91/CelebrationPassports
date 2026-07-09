using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class Role
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

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
