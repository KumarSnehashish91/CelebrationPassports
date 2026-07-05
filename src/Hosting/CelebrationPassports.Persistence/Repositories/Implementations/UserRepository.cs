using CelebrationPassports.Persistence.Context;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CelebrationPassports.Persistence.Repositories.Implementations;

public class UserRepository
    : GenericRepository<User>, IUserRepository
{
    public UserRepository(
        CelebrationPassportsDbContext context)
        : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    //public async Task<User?> GetByMobileAsync(string mobile)
    //{
    //    return await _context.Users
    //        .FirstOrDefaultAsync(x => x.Mobile == mobile);
    //}

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(x => x.Email == email);
    }

    //public async Task<bool> MobileExistsAsync(string mobile)
    //{
    //    return await _context.Users
    //        .AnyAsync(x => x.Mobile == mobile);
    //}
}