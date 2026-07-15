using CelebrationPassports.Web.Models.Passports;

namespace CelebrationPassports.Web.Models.People;

public class PassportPeopleGroupViewModel
{
    public Guid PassportId { get; set; }

    public string PassportTitle { get; set; } = string.Empty;

    public List<PassportPersonViewModel> People { get; set; } = new();
}
