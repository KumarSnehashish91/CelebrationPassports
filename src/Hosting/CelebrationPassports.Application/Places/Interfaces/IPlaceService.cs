using CelebrationPassports.Application.Places.DTOs;

namespace CelebrationPassports.Application.Places.Interfaces;

public interface IPlaceService
{
    Task<IReadOnlyList<PlaceDto>> SearchAsync(string query);

    Task<PlaceDto> CreateAsync(CreatePlaceRequest request);

    Task<PlaceDto> GetByIdAsync(Guid id);
}
