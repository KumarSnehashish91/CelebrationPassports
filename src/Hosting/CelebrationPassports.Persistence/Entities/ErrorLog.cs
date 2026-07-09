using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class ErrorLog
{
    public Guid ErrorLogId { get; set; }

    public Guid? CorrelationId { get; set; }

    public Guid? UserId { get; set; }

    public string? ExceptionType { get; set; }

    public string? Message { get; set; }

    public string? StackTrace { get; set; }

    public string? Source { get; set; }

    public string? InnerException { get; set; }

    public string? Api { get; set; }

    public string? HttpMethod { get; set; }

    public string? Ipaddress { get; set; }

    public string? Browser { get; set; }

    public DateTime? CreatedOn { get; set; }
}
