namespace CelebrationPassports.Persistence.Entities;

public class Place
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
