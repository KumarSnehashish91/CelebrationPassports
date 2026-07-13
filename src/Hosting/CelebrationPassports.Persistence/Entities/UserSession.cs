using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class UserSession : BaseEntity
{

    public Guid UserId { get; set; }

    public string? AccessToken { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime RefreshTokenExpiryOn { get; set; }

    public string? DeviceName { get; set; }

    public string? DeviceType { get; set; }

    public string? Browser { get; set; }

    public string? OperatingSystem { get; set; }

    public string? Ipaddress { get; set; }

    public string? UserAgent { get; set; }

    public bool IsActive { get; set; }

    public DateTime LoggedInOn { get; set; }

    public DateTime? LastActivityOn { get; set; }

    public DateTime? LoggedOutOn { get; set; }

    public DateTime? RevokedOn { get; set; }

    public string? RevokedReason { get; set; }

    

    public virtual User User { get; set; } = null!;
}
