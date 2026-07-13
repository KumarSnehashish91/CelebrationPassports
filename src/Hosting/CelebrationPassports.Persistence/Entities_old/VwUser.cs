using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class VwUser
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public int StatusId { get; set; }

    public DateTime? EmailVerifiedOn { get; set; }

    public DateTime? LastPasswordChangedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? DisplayName { get; set; }

    public string? MobileNumber { get; set; }
}
