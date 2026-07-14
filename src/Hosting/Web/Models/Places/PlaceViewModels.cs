using System.ComponentModel.DataAnnotations;

namespace CelebrationPassports.Web.Models.Places;

public class PlaceSearchResultViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? Country { get; set; }
}

public class PlaceDetailViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? Notes { get; set; }
}

public class CreatePlaceViewModel
{
    [Required(ErrorMessage = "Venue / place name is required.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Address { get; set; }

    [Required(ErrorMessage = "City is required.")]
    [StringLength(200)]
    public string City { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [Required(ErrorMessage = "Country is required.")]
    [StringLength(200)]
    public string Country { get; set; } = "India";

    [StringLength(200)]
    public string? Notes { get; set; }

    // Only actually collected today by the Settings "Home Location" form — trip
    // detection needs real coordinates and there's no geocoding service to derive them
    // from a name/city, so the user enters them directly there. Null everywhere else.
    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
