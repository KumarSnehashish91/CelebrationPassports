namespace CelebrationPassports.Web.Models.Passports;

public class PassportListItemViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int PeopleCount { get; set; }

    public bool IsOwner { get; set; }
}
