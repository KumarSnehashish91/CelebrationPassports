using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwVisitor
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

    public Guid? WaitlistId { get; set; }

    public string? FullName { get; set; }

    public string? EmailAddress { get; set; }

    public string? MobileNumber { get; set; }

    public string? InvitationStatus { get; set; }

    public string? InterestedPlatform { get; set; }

    public Guid? UserId { get; set; }

    public string? RegisteredEmail { get; set; }

    public int? StatusId { get; set; }

    public DateTime CreatedOn { get; set; }
}
