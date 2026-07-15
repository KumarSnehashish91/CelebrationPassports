namespace CelebrationPassports.Web.Models.Expenses;

public class CategoryBudgetViewModel
{
    public int Category { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal BudgetedAmount { get; set; }

    public decimal SpentAmount { get; set; }

    public int PercentSpent => BudgetedAmount <= 0
        ? 0
        : (int)Math.Min(100, Math.Round(SpentAmount / BudgetedAmount * 100));

    public bool IsOverBudget => BudgetedAmount > 0 && SpentAmount > BudgetedAmount;
}
