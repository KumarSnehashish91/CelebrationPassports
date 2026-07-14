using CelebrationPassports.Persistence.Enums;

namespace CelebrationPassports.Persistence.Entities;

public class PassportBook
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public PassportBookStatus Status { get; set; }

    public Guid? CoverMediaId { get; set; }

    public int? PageCount { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual Media? CoverMedia { get; set; }

    public virtual ICollection<PassportBookChapter> BookChapters { get; set; } = new List<PassportBookChapter>();
}
