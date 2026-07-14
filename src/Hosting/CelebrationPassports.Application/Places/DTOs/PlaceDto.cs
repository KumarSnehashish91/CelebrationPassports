namespace CelebrationPassports.Application.Places.DTOs;

public class PlaceDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Notes { get; set; }
}
