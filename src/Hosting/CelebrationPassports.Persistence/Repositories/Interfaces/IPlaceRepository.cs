using CelebrationPassports.Persistence.Entities;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IPlaceRepository : IGenericRepository<Place>
{
    Task<IReadOnlyList<Place>> SearchAsync(string query, int take);
}
