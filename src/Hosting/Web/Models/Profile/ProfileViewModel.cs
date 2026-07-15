namespace CelebrationPassports.Web.Models.Profile;

public class ProfileViewModel
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public string Email { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? MobileNumber { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public DateTime CreatedOn { get; set; }
}
