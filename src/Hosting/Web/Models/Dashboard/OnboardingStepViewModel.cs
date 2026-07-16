namespace CelebrationPassports.Web.Models.Dashboard;

public class OnboardingStepViewModel
{
    public int Number { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsDone { get; set; }
}
