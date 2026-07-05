using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Repositories.Interfaces;

namespace CelebrationPassports.Persistence.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly CelebrationPassportsDbContext _context;

    public UnitOfWork(CelebrationPassportsDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}