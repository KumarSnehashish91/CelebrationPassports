using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? MobileNumber { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public Guid? AvatarMediaId { get; set; }

    // Reference point for AI trip detection — a batch of uploaded photos taken far from
    // here is treated as a likely trip. Unset by default; detection simply doesn't run
    // until the user sets one (see Settings).
    public Guid? HomePlaceId { get; set; }

    //Navigation
    public virtual User User { get; set; } = null!;

    public virtual Media? AvatarMedia { get; set; }

    public virtual Place? HomePlace { get; set; }
}
