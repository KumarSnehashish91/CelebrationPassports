using CelebrationPassports.Application.Authentication.DTOs.RequestDTO;
using CelebrationPassports.Application.Authentication.DTOs.ResponseDTO;
using CelebrationPassports.Application.Authentication.Interfaces;
using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Implementations;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CelebrationPassports.Application.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserLoginHistoryRepository _userLoginHistoryRepository;
    private readonly IUserSessionRepository _userLogout;
    public AuthenticationService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IUserLoginHistoryRepository userLoginHistory,
        IUserSessionRepository userLogout)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _userLoginHistoryRepository = userLoginHistory;
        _userLogout=    userLogout;
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
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.EmailAddress);

        if (user == null)
        {
           
            throw new UnauthorizedAccessException("Invalid email address or password.");
        }

        bool passwordMatched = _passwordHasher.VerifyPassword(request.Password,user.PasswordHash);

        if (!passwordMatched)
        {
            user.FailedLoginAttempts++;

            await SaveLoginHistoryAsync(
                user.Id,
                false,
                "Invalid password");

            //await _unitOfWork.SaveChangesAsync();

            throw new UnauthorizedAccessException("Invalid email address or password.");
        }

        if (user.IsLocked)
        {
            await SaveLoginHistoryAsync(
                user.Id,
                false,
                "Account locked");

            throw new UnauthorizedAccessException("Account is locked.");
        }

        if (user.IsDeleted)
        {
            await SaveLoginHistoryAsync(
                user.Id,
                false,
                "Account deleted");

            throw new UnauthorizedAccessException("Account is deleted.");
        }

        user.LastLoginOn = DateTime.UtcNow;
        user.LastSeenOn = DateTime.UtcNow;
        user.FailedLoginAttempts = 0;

        await SaveLoginHistoryAsync(
            user.Id,
            true,
            null);

        await SaveUserSessionAsync(
           user.Id,
           true,
           null);

        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.Email
        };
    }

    public Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        throw new NotImplementedException();
    }

    private async Task SaveLoginHistoryAsync(
    Guid userId,
    bool isSuccessful,
    string? failureReason)
    {
        var loginHistory = new UserLoginHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LoginOn = DateTime.UtcNow,
            IsLoggedIn = isSuccessful,
            FailureReason = failureReason,
            IpAddress = "",
            UserAgent = ""
        };

        await _userLoginHistoryRepository.AddAsync(loginHistory);       

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task SaveUserSessionAsync(
    Guid userId,
    bool isSuccessful,
    string? failureReason)
    {
        var _userSession = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,            
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            LoggedInOn = DateTime.UtcNow.AddHours(1),
            RefreshTokenExpiryOn=DateTime.UtcNow.AddHours(1),
            RefreshToken = Guid.NewGuid().ToString(),
        };
        

        await _userLogout.AddAsync(_userSession);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task LogoutAsync(LogoutRequest request)
    {
        
        var session = await _userLogout.GetActiveSessionByUserIdAsync(request.UserId);
        var _userSessionLogout = new UserSession
        {
            
            UserId = session.UserId,
            IsActive = false,
            LoggedOutOn = DateTime.UtcNow,
            RevokedOn = null,
            RevokedReason = "User Logged Out",
            LoggedInOn = DateTime.UtcNow.AddHours(1),
            CreatedOn = DateTime.UtcNow,
            RefreshTokenExpiryOn = DateTime.UtcNow.AddHours(1),
            RefreshToken = Guid.NewGuid().ToString(),
        };
        await _userLogout.AddAsync(_userSessionLogout);

        await _unitOfWork.SaveChangesAsync();
    }
}