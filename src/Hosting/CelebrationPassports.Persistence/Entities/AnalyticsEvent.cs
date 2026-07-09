using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class AnalyticsEvent
{
    public Guid AnalyticsEventId { get; set; }

    public Guid? VisitorSessionId { get; set; }

    public Guid? VisitorId { get; set; }

    public Guid? UserId { get; set; }

    public string EventCategory { get; set; } = null!;

    public string EventName { get; set; } = null!;

    public string? EntityName { get; set; }

    public Guid? EntityId { get; set; }

    public string? PageUrl { get; set; }

    public string? ElementId { get; set; }

    public string? ElementText { get; set; }

    public int? DurationMilliseconds { get; set; }

    public decimal? EventValue { get; set; }

    public string? AdditionalData { get; set; }

    public DateTime EventOccurredOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual Visitor? Visitor { get; set; }

    public virtual VisitorSession? VisitorSession { get; set; }
}
