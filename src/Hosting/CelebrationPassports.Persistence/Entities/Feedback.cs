using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class Feedback
{
    public Guid FeedbackId { get; set; }

    public Guid? VisitorId { get; set; }

    public Guid? VisitorSessionId { get; set; }

    public Guid? WaitlistId { get; set; }

    public Guid? UserId { get; set; }

    public byte Rating { get; set; }

    public byte? UserExperienceRating { get; set; }

    public byte? DesignRating { get; set; }

    public byte? EaseOfUseRating { get; set; }

    public byte? PerformanceRating { get; set; }

    public bool WouldRecommend { get; set; }

    public bool WouldJoinBeta { get; set; }

    public string? FavouriteFeature { get; set; }

    public string? MissingFeature { get; set; }

    public string? ImprovementSuggestion { get; set; }

    public string? Comments { get; set; }

    public string FeedbackSource { get; set; } = null!;

    public string Status { get; set; } = null!;

    public Guid? ReviewedBy { get; set; }

    public DateTime? ReviewedOn { get; set; }

    public string? AdminRemarks { get; set; }

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

    public virtual Waitlist? Waitlist { get; set; }
}
