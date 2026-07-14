namespace CelebrationPassports.Web.Models.Account;

public class AuthResult
{
    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }

    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresOn { get; set; }

    public Guid SessionId { get; set; }
}
