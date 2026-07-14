namespace CelebrationPassports.Persistence.Entities;

public class Story
{
    public Guid Id { get; set; }

    public Guid PassportId { get; set; }

    public string Title { get; set; } = string.Empty;

    // AI-inferred from Chapters/Media, user-editable, independent of any source Event.
    public Guid? PlaceId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid? CoverMediaId { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public virtual Passport Passport { get; set; } = null!;

    public virtual Place? Place { get; set; }

    public virtual Media? CoverMedia { get; set; }

    public virtual User? DeletedByUser { get; set; }

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    // Inverse of Event.StoryId — a Story has at most one Event pointing at it in practice,
    // but the relationship is expressed as a single one-directional FK on Event (per schema doc).
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
