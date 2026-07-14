using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Repositories.Implementations;

public class PlaceRepository : GenericRepository<Place>, IPlaceRepository
{
    public PlaceRepository(CelebrationPassportsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Place>> SearchAsync(string query, int take)
    {
        return await _dbcontext.Places
            .AsNoTracking()
            .Where(p => EF.Functions.ILike(p.Name, $"%{query}%")
                || (p.City != null && EF.Functions.ILike(p.City, $"%{query}%"))
                || (p.Country != null && EF.Functions.ILike(p.Country, $"%{query}%")))
            .OrderBy(p => p.Name)
            .Take(take)
            .ToListAsync();
    }
}
