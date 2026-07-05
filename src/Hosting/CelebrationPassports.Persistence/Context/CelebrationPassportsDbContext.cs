using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Context;

public class CelebrationPassportsDbContext : DbContext
{
    public CelebrationPassportsDbContext(
        DbContextOptions<CelebrationPassportsDbContext> options)
        : base(options)
    {
    }
}