using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwWaitlist
{
    public Guid WaitlistId { get; set; }

    public string FullName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string? MobileNumber { get; set; }

    public string? Country { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public string? CompanyName { get; set; }

    public string? Occupation { get; set; }

    public string InterestedPlatform { get; set; } = null!;

    public string? ReferralSource { get; set; }

    public string InvitationStatus { get; set; } = null!;

    public string? InvitationBatch { get; set; }

    public DateTime? InvitationSentOn { get; set; }

    public DateTime? RegisteredOn { get; set; }

    public int? TotalVisits { get; set; }

    public int? TotalSessions { get; set; }

    public int? TotalPageViews { get; set; }

    public DateTime? LastVisitOn { get; set; }

    public string? RegisteredEmail { get; set; }

    public DateTime CreatedOn { get; set; }
}
