using CelebrationPassports.Application.Authentication.DTOs.RequestDTO;
using CelebrationPassports.Application.Authentication.DTOs.ResponseDTO;
using CelebrationPassports.Application.Authentication.Interfaces;
using CelebrationPassports.Application.Exceptions;
using CelebrationPassports.Infrastructure.Authentication.Interfaces;
using CelebrationPassports.Persistence.Entities;
using CelebrationPassports.Persistence.Repositories.Implementations;
using CelebrationPassports.Persistence.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CelebrationPassports.Application.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserLoginHistoryRepository _userLoginHistoryRepository;
    private readonly IUserSessionRepository _userLogout;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ITokenService _tokenService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    public AuthenticationService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IUserLoginHistoryRepository userLoginHistory,
        IUserSessionRepository userLogout,
        IUserProfileRepository userProfileRepository,
        ITokenService tokenService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _userLoginHistoryRepository = userLoginHistory;
        _userLogout=    userLogout;
        _userProfileRepository = userProfileRepository;
        _tokenService = tokenService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }



    // Methods will come here

    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest request)
    {
        await _registerValidator.ValidateAndThrowAsync(request);

        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new DuplicateEmailException(request.Email);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash,
            StatusId = 1,
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false
        };

        await _userRepository.AddAsync(user);

        var profile = new UserProfile
        {
            UserId = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DisplayName = $"{request.FirstName} {request.LastName}",
            CreatedOn = DateTime.UtcNow
        };

        await _userProfileRepository.AddAsync(profile);

        var sessionId = Guid.NewGuid();
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, sessionId);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiresOn = _tokenService.GetAccessTokenExpiry();

        await SaveUserSessionAsync(user.Id, sessionId, refreshToken);

        await _unitOfWork.SaveChangesAsync();

        return new AuthenticationResponse
        {
            Id = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresOn = expiresOn,
            SessionId = sessionId
        };
    }
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        await _loginValidator.ValidateAndThrowAsync(request);

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

        var sessionId = Guid.NewGuid();
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, sessionId);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiresOn = _tokenService.GetAccessTokenExpiry();

        await SaveUserSessionAsync(user.Id, sessionId, refreshToken);

        await _unitOfWork.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresOn = expiresOn,
            SessionId = sessionId
        };
    }

    public async Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var session = await _userLogout.GetByRefreshTokenAsync(request.RefreshToken);

        if (session == null
            || !session.IsActive
            || session.RevokedOn != null
            || session.RefreshTokenExpiryOn < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("The refresh token is invalid or has expired.");
        }

        var user = await _userRepository.GetByIdAsync(session.UserId);

        if (user == null || user.IsDeleted || user.IsLocked)
        {
            throw new UnauthorizedAccessException("The refresh token is invalid or has expired.");
        }

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, session.Id);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var expiresOn = _tokenService.GetAccessTokenExpiry();

        session.RefreshToken = newRefreshToken;
        session.RefreshTokenExpiryOn = _tokenService.GetRefreshTokenExpiry();
        session.LastActivityOn = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return new AuthenticationResponse
        {
            Id = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresOn = expiresOn,
            SessionId = session.Id
        };
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
    Guid sessionId,
    string refreshToken)
    {
        var _userSession = new UserSession
        {
            Id = sessionId,
            UserId = userId,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            LoggedInOn = DateTime.UtcNow,
            RefreshTokenExpiryOn = _tokenService.GetRefreshTokenExpiry(),
            RefreshToken = refreshToken,
        };


        await _userLogout.AddAsync(_userSession);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task LogoutAsync(Guid userId, LogoutRequest request)
    {

        var _userSessionLogout = await _userLogout.GetActiveSessionByUserIdAsync(userId, request.SessionId);

        if (_userSessionLogout == null)
        {
            throw new NotFoundException("The session was not found.");
        }

        _userSessionLogout.IsActive = false;
        _userSessionLogout.LoggedOutOn = DateTime.UtcNow;
        _userSessionLogout.RevokedOn = null;
        _userSessionLogout.RevokedReason = "User Logged Out";
        _userSessionLogout.LoggedOutOn = DateTime.UtcNow;
        _userSessionLogout.ModifiedOn = DateTime.UtcNow;


        //Update Login History
        var loginHistory =
     await _userLoginHistoryRepository
         .GetLoginUserHistoryAsync(userId);

        if (loginHistory != null)
        {
            loginHistory.IsLoggedIn = false;
            loginHistory.LogoutOn = DateTime.UtcNow;
            loginHistory.ModifiedBy = loginHistory.UserId;
            loginHistory.ModifiedOn = DateTime.UtcNow;
        }


        await _unitOfWork.SaveChangesAsync();
    }
}
