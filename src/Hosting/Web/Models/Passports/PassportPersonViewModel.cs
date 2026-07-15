namespace CelebrationPassports.Web.Models.Passports;

public class PassportPersonViewModel
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
