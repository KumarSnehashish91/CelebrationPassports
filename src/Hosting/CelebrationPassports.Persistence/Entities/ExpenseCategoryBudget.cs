using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

// One row per (EventId, Category) — the budgeted amount a user has set aside for that
// category on a trip/event. Actual spend is derived live from Expense rows rather than
// cached here, so the two can never drift out of sync.
public class ExpenseCategoryBudget
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public ExpenseCategory Category { get; set; }

    public decimal BudgetedAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;
}
