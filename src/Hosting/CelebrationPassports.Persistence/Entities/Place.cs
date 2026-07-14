namespace CelebrationPassports.Persistence.Entities;

public class Place
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Story> Stories { get; set; } = new List<Story>();

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual ICollection<PassportStamp> PassportStamps { get; set; } = new List<PassportStamp>();
}
