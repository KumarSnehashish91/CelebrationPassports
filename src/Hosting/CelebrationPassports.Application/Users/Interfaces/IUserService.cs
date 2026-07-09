using CelebrationPassports.Application.Users.DTOs;

namespace CelebrationPassports.Application.Users.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto?> GetByEmailAsync(string email);

    Task<IReadOnlyList<UserDto>> GetAllAsync();
}