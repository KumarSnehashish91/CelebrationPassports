using CelebrationPassports.Web.Models.Places;

namespace CelebrationPassports.Web.Interfaces;

public interface IPlaceService
{
    Task<List<PlaceSearchResultViewModel>> SearchAsync(string query);

    Task<Guid?> CreateAsync(CreatePlaceViewModel model);

    Task<PlaceDetailViewModel?> GetByIdAsync(Guid id);
}
