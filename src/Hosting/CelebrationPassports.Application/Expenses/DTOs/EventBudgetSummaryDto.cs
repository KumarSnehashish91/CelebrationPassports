namespace CelebrationPassports.Application.Expenses.DTOs;

public class EventBudgetSummaryDto
{
    public Guid EventId { get; set; }

    public List<CategoryBudgetLineDto> Categories { get; set; } = new();

    public decimal TotalBudgeted { get; set; }

    public decimal TotalSpent { get; set; }
}
