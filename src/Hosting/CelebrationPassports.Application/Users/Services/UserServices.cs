using CelebrationPassports.Application.Users.DTOs;
using CelebrationPassports.Application.Users.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;

namespace CelebrationPassports.Application.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            StatusId = user.StatusId,
            EmailVerifiedOn = user.EmailVerifiedOn
        };
    }
}