using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwPageView
{
    public Guid PageViewId { get; set; }

    public Guid VisitorSessionId { get; set; }

    public Guid VisitorId { get; set; }

    public string PageUrl { get; set; } = null!;

    public string? PageTitle { get; set; }

    public DateTime EnteredOn { get; set; }

    public DateTime? ExitedOn { get; set; }

    public int DurationSeconds { get; set; }

    public decimal? ScrollPercentage { get; set; }

    public int ClickCount { get; set; }

    public bool IsLandingPage { get; set; }

    public bool IsExitPage { get; set; }
}
