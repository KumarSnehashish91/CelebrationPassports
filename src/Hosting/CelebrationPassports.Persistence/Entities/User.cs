using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int StatusId { get; set; }

    public DateTime? EmailVerifiedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public DateTime? LastPasswordChangedOn { get; set; }

    public DateTime? LastLoginOn { get; set; }

    public DateTime? LastSeenOn { get; set; }

    public int FailedLoginAttempts { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public virtual ICollection<AnalyticsEvent> AnalyticsEvents { get; set; } = new List<AnalyticsEvent>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Passport> PassportCreatedByNavigations { get; set; } = new List<Passport>();

    public virtual ICollection<Passport> PassportDeletedByNavigations { get; set; } = new List<Passport>();

    public virtual ICollection<PassportMember> PassportMembers { get; set; } = new List<PassportMember>();

    public virtual ICollection<Passport> PassportModifiedByNavigations { get; set; } = new List<Passport>();

    public virtual ICollection<PassportMoment> PassportMomentCreatedByNavigations { get; set; } = new List<PassportMoment>();

    public virtual ICollection<PassportMoment> PassportMomentDeletedByNavigations { get; set; } = new List<PassportMoment>();

    public virtual ICollection<PassportMoment> PassportMomentModifiedByNavigations { get; set; } = new List<PassportMoment>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Status Status { get; set; } = null!;

    public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();

    public virtual ICollection<UserLoginHistory> UserLoginHistories { get; set; } = new List<UserLoginHistory>();

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

    public virtual ICollection<Visitor> Visitors { get; set; } = new List<Visitor>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
