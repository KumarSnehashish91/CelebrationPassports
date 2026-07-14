using CelebrationPassports.Application.Authentication.DTOs.RequestDTO;
using CelebrationPassports.Application.Authentication.DTOs.ResponseDTO;
using CelebrationPassports.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> RegisterAsync(RegisterRequest request);

        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task LogoutAsync(Guid userId, LogoutRequest request);
    }
}
