using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class SessionTrafficSourceInfo
{
    public Guid SessionTrafficSourceInfoId { get; set; }

    public Guid VisitorSessionId { get; set; }

    public string? Referrer { get; set; }

    public string? Source { get; set; }

    public string? Medium { get; set; }

    public string? Campaign { get; set; }

    public string? Content { get; set; }

    public string? Term { get; set; }

    public string? Utmsource { get; set; }

    public string? Utmmedium { get; set; }

    public string? Utmcampaign { get; set; }

    public string? Utmcontent { get; set; }

    public string? Utmterm { get; set; }

    public DateTime? CreatedOn { get; set; }
}
