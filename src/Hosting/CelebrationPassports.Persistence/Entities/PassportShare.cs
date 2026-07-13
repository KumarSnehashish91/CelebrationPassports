namespace CelebrationPassports.Persistence.Entities;

public class PassportShare
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string ShareToken { get; set; } = string.Empty;

    public DateTime? Expiry { get; set; }

    // Store a password hash here when password-protected sharing is implemented.
    public string? Password { get; set; }

    public int ViewCount { get; set; }

    public DateTime? RevokedAt { get; set; }

    public virtual Passport Passport { get; set; } = null!;
}
