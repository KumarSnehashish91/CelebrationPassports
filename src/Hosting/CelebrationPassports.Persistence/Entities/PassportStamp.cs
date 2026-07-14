namespace CelebrationPassports.Persistence.Entities;

public class PassportStamp
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public Guid PlaceId { get; set; }

    public Guid? SourceChapterId { get; set; }

    public DateTime EarnedOn { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual Place Place { get; set; } = null!;

    public virtual Chapter? SourceChapter { get; set; }
}
