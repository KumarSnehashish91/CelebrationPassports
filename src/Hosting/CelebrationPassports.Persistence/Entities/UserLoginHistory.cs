using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class UserLoginHistory
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime LoginOn { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public bool IsLoggedIn { get; set; }

    public string? FailureReason { get; set; }

    public virtual User User { get; set; } = null!;
}
