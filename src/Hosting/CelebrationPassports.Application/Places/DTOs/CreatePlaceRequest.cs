namespace CelebrationPassports.Application.Places.DTOs;

public class CreatePlaceRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string City { get; set; } = string.Empty;

    public string? PostalCode { get; set; }

    public string Country { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Notes { get; set; }
}
