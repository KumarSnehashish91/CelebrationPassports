using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class Visitor
{
    public Guid VisitorId { get; set; }

    public string AnonymousId { get; set; } = null!;

    public Guid? RegisteredUserId { get; set; }

    public DateTime FirstVisitOn { get; set; }

    public DateTime LastVisitOn { get; set; }

    public int TotalVisits { get; set; }

    public int TotalSessions { get; set; }

    public int TotalPageViews { get; set; }

    public int TotalDurationSeconds { get; set; }

    public bool IsBetaInterested { get; set; }

    public DateTime? JoinedWaitlistOn { get; set; }

    public bool IsRegistered { get; set; }

    public DateTime? RegisteredOn { get; set; }

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

    public virtual User? RegisteredUser { get; set; }

    public virtual ICollection<VisitorSession> VisitorSessions { get; set; } = new List<VisitorSession>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
