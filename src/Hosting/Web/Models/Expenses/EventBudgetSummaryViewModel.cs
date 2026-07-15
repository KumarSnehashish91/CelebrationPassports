namespace CelebrationPassports.Web.Models.Expenses;

public class EventBudgetSummaryViewModel
{
    public Guid EventId { get; set; }

    public List<CategoryBudgetViewModel> Categories { get; set; } = new();

    public decimal TotalBudgeted { get; set; }

    public decimal TotalSpent { get; set; }

    public int PercentSpent => TotalBudgeted <= 0
        ? 0
        : (int)Math.Min(100, Math.Round(TotalSpent / TotalBudgeted * 100));
}
