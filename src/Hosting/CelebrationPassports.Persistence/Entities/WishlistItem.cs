namespace CelebrationPassports.Persistence.Entities;

public class WishlistItem
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Category { get; set; }

    public decimal Price { get; set; }

    public string Url { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }
}
