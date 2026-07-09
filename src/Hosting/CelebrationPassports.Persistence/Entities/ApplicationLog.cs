using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class ApplicationLog
{
    public Guid ApplicationLogId { get; set; }

    public string? LogLevel { get; set; }

    public string? Category { get; set; }

    public string? Source { get; set; }

    public string? Message { get; set; }

    public string? Exception { get; set; }

    public string? StackTrace { get; set; }

    public string? RequestPath { get; set; }

    public string? HttpMethod { get; set; }

    public string? Ipaddress { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? CreatedOn { get; set; }
}
