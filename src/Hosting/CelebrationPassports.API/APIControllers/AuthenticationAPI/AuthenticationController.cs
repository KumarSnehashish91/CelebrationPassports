using CelebrationPassports.Application.Authentication.DTOs.RequestDTO;
using CelebrationPassports.Application.Authentication.DTOs.ResponseDTO;
using CelebrationPassports.Application.Authentication.Interfaces;
using CelebrationPassports.Application.Users.DTOs;
using CelebrationPassports.Application.Users.Interfaces;
using CelebrationPassports.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.AuthenticationAPI;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(
        IAuthenticationService authenticationService )
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _authenticationService.RegisterAsync(request);

        return CreatedAtAction(
            nameof(Register),
            new { id = response.Id },
            response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authenticationService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized();
        }
        return Ok(response);
    }
    [HttpPost("logout")]
    public async Task Logout(LogoutRequest request)
    {
        // Implement logout logic here, e.g., invalidate the refresh token
        await _authenticationService.LogoutAsync(request);
        //return Ok(new { Message = "Logged out successfully." });
    }

   
}