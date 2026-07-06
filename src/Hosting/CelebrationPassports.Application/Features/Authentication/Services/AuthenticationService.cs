using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Application.Features.Authentication.DTOs;
using CelebrationPassports.Application.Features.Authentication.Interfaces;
using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CelebrationPassports.Application.Features.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public AuthenticationService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }



    // Methods will come here
    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new DuplicateEmailException(request.Email);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            StatusId = 1,
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false
        };

        await _userRepository.AddAsync(user);

        await _unitOfWork.SaveChangesAsync();

        return new AuthenticationResponse
        {
            Id = user.Id,
            Email = user.Email
        };
    }
    public Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        throw new NotImplementedException();
    }
}