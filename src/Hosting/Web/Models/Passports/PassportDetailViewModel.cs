namespace CelebrationPassports.Web.Models.Passports;

public class PassportDetailViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public Guid OwnerUserId { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsOwner { get; set; }

    public List<PassportPersonViewModel> People { get; set; } = new();
}
