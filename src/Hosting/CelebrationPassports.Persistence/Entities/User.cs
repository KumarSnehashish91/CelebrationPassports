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


    public DateTime? LastPasswordChangedOn { get; set; }

    public DateTime? LastLoginOn { get; set; }

    public DateTime? LastSeenOn { get; set; }

    public int FailedLoginAttempts { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    

    public virtual ICollection<UserLoginHistory> UserLoginHistories { get; set; } = new List<UserLoginHistory>();

    public virtual UserProfile? UserProfile { get; set; }

   

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();

    public virtual ICollection<Passport> OwnedPassports { get; set; } = new List<Passport>();

    public virtual ICollection<Passport> DeletedPassports { get; set; } = new List<Passport>();

    public virtual ICollection<PassportPerson> PassportPeople { get; set; } = new List<PassportPerson>();

    public virtual ICollection<PassportPerson> DeletedPassportPeople { get; set; } = new List<PassportPerson>();

    public virtual ICollection<PassportInvitation> SentPassportInvitations { get; set; } = new List<PassportInvitation>();

    public virtual ICollection<PassportOwnershipHistory> OwnershipTransfersFrom { get; set; } = new List<PassportOwnershipHistory>();

    public virtual ICollection<PassportOwnershipHistory> OwnershipTransfersTo { get; set; } = new List<PassportOwnershipHistory>();

    public virtual ICollection<PassportOwnershipHistory> OwnershipTransfersPerformed { get; set; } = new List<PassportOwnershipHistory>();

    public virtual ICollection<Trip> DeletedTrips { get; set; } = new List<Trip>();

    public virtual ICollection<Chapter> DeletedChapters { get; set; } = new List<Chapter>();

  
}
