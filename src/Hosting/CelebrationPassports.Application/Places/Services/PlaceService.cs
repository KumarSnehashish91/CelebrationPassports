using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Places.DTOs;
using CelebrationPassports.Application.Places.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;

namespace CelebrationPassports.Application.Places.Services;

public class PlaceService : IPlaceService
{
    private readonly IPlaceRepository _placeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreatePlaceRequest> _createValidator;

    public PlaceService(
        IPlaceRepository placeRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreatePlaceRequest> createValidator)
    {
        _placeRepository = placeRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<IReadOnlyList<PlaceDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var places = await _placeRepository.SearchAsync(query, take: 10);

        return places.Select(MapToDto).ToList();
    }

    public async Task<PlaceDto> CreateAsync(CreatePlaceRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var place = new Place
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Notes = request.Notes
        };

        await _placeRepository.AddAsync(place);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(place);
    }

    public async Task<PlaceDto> GetByIdAsync(Guid id)
    {
        var place = await _placeRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Place not found.");

        return MapToDto(place);
    }

    private static PlaceDto MapToDto(Place place) => new()
    {
        Id = place.Id,
        Name = place.Name,
        Address = place.Address,
        City = place.City,
        PostalCode = place.PostalCode,
        Country = place.Country,
        Latitude = place.Latitude,
        Longitude = place.Longitude,
        Notes = place.Notes
    };
}
