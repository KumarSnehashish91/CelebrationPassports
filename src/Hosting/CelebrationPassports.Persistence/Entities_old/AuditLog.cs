using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class AuditLog
{
    public Guid AuditLogId { get; set; }

    public Guid? UserId { get; set; }

    public string Module { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public Guid? EntityId { get; set; }

    public string Action { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? Ipaddress { get; set; }

    public string? Browser { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedOn { get; set; }
}
