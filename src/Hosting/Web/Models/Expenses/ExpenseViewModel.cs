namespace CelebrationPassports.Web.Models.Expenses;

public class ExpenseViewModel
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    // Mirrors CelebrationPassports.Persistence.Enums.ExpenseCategory
    // (Stay=1, Travel=2, Food=3, Activities=4, Other=5).
    public int Category { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public DateOnly SpentOn { get; set; }

    public Guid CreatedByUserId { get; set; }

    public string CreatedByName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsMine { get; set; }
}
