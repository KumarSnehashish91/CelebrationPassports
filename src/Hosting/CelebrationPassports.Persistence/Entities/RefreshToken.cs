using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresOn { get; set; }

    public DateTime? RevokedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual User User { get; set; } = null!;
}
