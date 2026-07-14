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

    //Navigation
    public virtual User User { get; set; } = null!;

    public virtual Media? AvatarMedia { get; set; }
}
