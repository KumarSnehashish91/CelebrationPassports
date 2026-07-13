using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class UserDevice
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? DeviceName { get; set; }

    public string? DeviceType { get; set; }

    public string? DeviceIdentifier { get; set; }

    public DateTime? LastLoginOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
