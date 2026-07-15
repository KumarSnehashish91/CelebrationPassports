using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class Expense
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public ExpenseCategory Category { get; set; }

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public DateOnly SpentOn { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }
}
