namespace CelebrationPassports.Application.Expenses.DTOs;

public class CreateExpenseRequest
{
    public int Category { get; set; }

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public DateOnly? SpentOn { get; set; }
}
