using CelebrationPassports.Application.Features.Authentication.DTOs;
using CelebrationPassports.Application.Features.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(
        IAuthenticationService authenticationService)
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
}