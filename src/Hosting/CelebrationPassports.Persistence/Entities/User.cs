using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Entities;

[Index("StatusId", Name = "IX_Users_StatusId")]
[Index("Email", Name = "UQ_Users_Email", IsUnique = true)]
public partial class User
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(320)]
    public string Email { get; set; } = null!;

    [StringLength(500)]
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

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Passport> PassportCreatedByNavigations { get; set; } = new List<Passport>();

    [InverseProperty("DeletedByNavigation")]
    public virtual ICollection<Passport> PassportDeletedByNavigations { get; set; } = new List<Passport>();

    [InverseProperty("User")]
    public virtual ICollection<PassportMember> PassportMembers { get; set; } = new List<PassportMember>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Passport> PassportModifiedByNavigations { get; set; } = new List<Passport>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<PassportMoment> PassportMomentCreatedByNavigations { get; set; } = new List<PassportMoment>();

    [InverseProperty("DeletedByNavigation")]
    public virtual ICollection<PassportMoment> PassportMomentDeletedByNavigations { get; set; } = new List<PassportMoment>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<PassportMoment> PassportMomentModifiedByNavigations { get; set; } = new List<PassportMoment>();

    [InverseProperty("User")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [ForeignKey("StatusId")]
    [InverseProperty("Users")]
    public virtual Status Status { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();

    [InverseProperty("User")]
    public virtual ICollection<UserLoginHistory> UserLoginHistories { get; set; } = new List<UserLoginHistory>();

    [InverseProperty("User")]
    public virtual UserProfile? UserProfile { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    [InverseProperty("User")]
    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
}
