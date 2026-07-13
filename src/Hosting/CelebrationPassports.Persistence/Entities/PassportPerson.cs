using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class PassportPerson
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    // Null when the person has not created an account yet.
    public Guid? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public PassportPersonRole Role { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual User? DeletedByUser { get; set; }
}
