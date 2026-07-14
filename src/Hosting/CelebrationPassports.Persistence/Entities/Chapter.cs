namespace CelebrationPassports.Persistence.Entities;

public class Chapter
{
    public Guid Id { get; set; }

    public Guid StoryId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? PlaceId { get; set; }

    public Guid? CoverMediaId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Story Story { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual Place? Place { get; set; }

    public virtual Media? CoverMedia { get; set; }

    public virtual User? DeletedByUser { get; set; }

    public virtual ICollection<Media> Media { get; set; } = new List<Media>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    public virtual ICollection<PassportBookChapter> BookChapters { get; set; } = new List<PassportBookChapter>();

    public virtual ICollection<PassportStamp> PassportStamps { get; set; } = new List<PassportStamp>();
}
