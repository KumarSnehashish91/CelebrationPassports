using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class UserRole
{
    public Guid UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime AssignedOn { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
