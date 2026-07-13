using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwVisitorSession
{
    public Guid VisitorSessionId { get; set; }

    public Guid VisitorId { get; set; }

    public string AnonymousId { get; set; } = null!;

    public string SessionIdentifier { get; set; } = null!;

    public DateTime SessionStartOn { get; set; }

    public DateTime? SessionEndOn { get; set; }

    public int SessionDurationSeconds { get; set; }

    public DateTime? LastActivityOn { get; set; }

    public int TotalPageViews { get; set; }

    public int TotalClicks { get; set; }

    public bool IsBounce { get; set; }

    public bool TrailerPlayed { get; set; }

    public bool TrailerCompleted { get; set; }

    public int TrailerWatchDurationSeconds { get; set; }

    public string? LandingPage { get; set; }

    public string? ExitPage { get; set; }

    public string? Ipaddress { get; set; }

    public string? Country { get; set; }

    public string? CountryCode { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? TimeZone { get; set; }

    public string? Referrer { get; set; }

    public string? Source { get; set; }

    public string? Medium { get; set; }

    public string? Campaign { get; set; }

    public DateTime CreatedOn { get; set; }
}
