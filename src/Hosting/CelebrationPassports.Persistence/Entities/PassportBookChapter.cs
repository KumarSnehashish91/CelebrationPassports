namespace CelebrationPassports.Persistence.Entities;

public class PassportBookChapter
{
    public Guid Id { get; set; }

    public Guid BookId { get; set; }

    public Guid ChapterId { get; set; }

    public int DisplayOrder { get; set; }

    public virtual PassportBook Book { get; set; } = null!;

    public virtual Chapter Chapter { get; set; } = null!;
}
