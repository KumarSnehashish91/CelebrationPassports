using CelebrationPassports.Application.Features.Users.DTOs;

namespace CelebrationPassports.Application.Features.Users.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto?> GetByEmailAsync(string email);

    Task<IReadOnlyList<UserDto>> GetAllAsync();
}