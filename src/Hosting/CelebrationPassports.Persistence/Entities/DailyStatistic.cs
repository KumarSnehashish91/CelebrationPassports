using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class DailyStatistic
{
    public Guid DailyStatisticsId { get; set; }

    public DateOnly StatisticsDate { get; set; }

    public int? Visitors { get; set; }

    public int? UniqueVisitors { get; set; }

    public int? ReturningVisitors { get; set; }

    public int? WaitlistRegistrations { get; set; }

    public int? UserRegistrations { get; set; }

    public int? PassportsCreated { get; set; }

    public int? MomentsCreated { get; set; }

    public int? MediaUploaded { get; set; }

    public int? FeedbackReceived { get; set; }

    public decimal? AverageSessionDuration { get; set; }

    public decimal? BounceRate { get; set; }

    public decimal? AverageRating { get; set; }

    public DateTime? CreatedOn { get; set; }
}
