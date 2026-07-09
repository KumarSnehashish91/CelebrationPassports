using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VisitorSession
{
    public Guid VisitorSessionId { get; set; }

    public Guid VisitorId { get; set; }

    public string SessionIdentifier { get; set; } = null!;

    public DateTime SessionStartOn { get; set; }

    public DateTime? SessionEndOn { get; set; }

    public int SessionDurationSeconds { get; set; }

    public DateTime? LastActivityOn { get; set; }

    public bool IsBounce { get; set; }

    public int TotalPageViews { get; set; }

    public int TotalClicks { get; set; }

    public bool TrailerPlayed { get; set; }

    public bool TrailerCompleted { get; set; }

    public int TrailerWatchDurationSeconds { get; set; }

    public string? LandingPage { get; set; }

    public string? ExitPage { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<AnalyticsEvent> AnalyticsEvents { get; set; } = new List<AnalyticsEvent>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<PageView> PageViews { get; set; } = new List<PageView>();

    public virtual SessionLocationInfo? SessionLocationInfo { get; set; }

    public virtual Visitor Visitor { get; set; } = null!;

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
