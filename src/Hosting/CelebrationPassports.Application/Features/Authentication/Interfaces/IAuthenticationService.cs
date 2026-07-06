using CelebrationPassports.Application.Features.Authentication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelebrationPassports.Application.Features.Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> RegisterAsync(RegisterRequest request);

        Task<AuthenticationResponse> LoginAsync(LoginRequest request);

        Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
