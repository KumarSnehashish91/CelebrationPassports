using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwFeedback
{
    public Guid FeedbackId { get; set; }

    public Guid? VisitorId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? WaitlistId { get; set; }

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

    public string Status { get; set; } = null!;

    public string FeedbackSource { get; set; } = null!;

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public DateTime CreatedOn { get; set; }
}
