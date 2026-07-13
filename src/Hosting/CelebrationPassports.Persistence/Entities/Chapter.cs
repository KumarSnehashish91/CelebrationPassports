namespace CelebrationPassports.Persistence.Entities;

public class Chapter
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? PlaceId { get; set; }

    public Guid? TripId { get; set; }

    // The Media FK is added after Media exists, to avoid the circular dependency.
    public Guid? CoverMediaId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual Place? Place { get; set; }

    public virtual Trip? Trip { get; set; }

    public virtual User? DeletedByUser { get; set; }
}
