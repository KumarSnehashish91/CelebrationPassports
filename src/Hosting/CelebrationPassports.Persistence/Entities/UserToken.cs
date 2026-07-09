using System;
using System.Collections.Generic;

namespace CelebrationPassports.Persistence.Entities;

public partial class UserToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenType { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpiresOn { get; set; }

    public DateTime? UsedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual User User { get; set; } = null!;
}
