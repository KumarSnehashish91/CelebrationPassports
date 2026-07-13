namespace CelebrationPassports.Persistence.Entities;

public class Trip
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid PlaceId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual Place Place { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
