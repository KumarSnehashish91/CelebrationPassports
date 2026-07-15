using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Application.Expenses.DTOs;

public class ExpenseDto
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public ExpenseCategory Category { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public DateOnly SpentOn { get; set; }

    public Guid CreatedByUserId { get; set; }

    public string CreatedByName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
