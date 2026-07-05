using CelebrationPassports.Persistence.Entities;

namespace CelebrationPassports.Persistence.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);

    //Task<User?> GetByMobileAsync(string mobile);

    Task<bool> EmailExistsAsync(string email);

    //Task<bool> MobileExistsAsync(string mobile);
}