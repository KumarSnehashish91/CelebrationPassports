using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class Waitlist
{
    public Guid WaitlistId { get; set; }

    public Guid? VisitorId { get; set; }

    public Guid? VisitorSessionId { get; set; }

    public Guid? UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string? MobileNumber { get; set; }

    public string? Country { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public string? Occupation { get; set; }

    public string? CompanyName { get; set; }

    public string InterestedPlatform { get; set; } = null!;

    public string? ReferralSource { get; set; }

    public bool WantsNewsletter { get; set; }

    public bool WantsBetaAccess { get; set; }

    public bool ConsentAccepted { get; set; }

    public string InvitationStatus { get; set; } = null!;

    public string? InvitationBatch { get; set; }

    public DateTime? InvitationSentOn { get; set; }

    public DateTime? RegisteredOn { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual User? User { get; set; }

    public virtual Visitor? Visitor { get; set; }

    public virtual VisitorSession? VisitorSession { get; set; }
}
