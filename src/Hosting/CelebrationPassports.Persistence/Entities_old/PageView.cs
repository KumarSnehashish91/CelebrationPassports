using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class PageView
{
    public Guid PageViewId { get; set; }

    public Guid VisitorSessionId { get; set; }

    public string PageUrl { get; set; } = null!;

    public string? PageTitle { get; set; }

    public DateTime EnteredOn { get; set; }

    public DateTime? ExitedOn { get; set; }

    public int DurationSeconds { get; set; }

    public decimal? ScrollPercentage { get; set; }

    public int ClickCount { get; set; }

    public bool IsLandingPage { get; set; }

    public bool IsExitPage { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual VisitorSession VisitorSession { get; set; } = null!;
}
