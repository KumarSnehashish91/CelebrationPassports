using System.Security.Claims;
using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUserProfileService _userProfileService;

    public ProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public async Task<IActionResult> Index()
    {
        var profile = await _userProfileService.GetProfileAsync();

        if (profile is null)
        {
            return NotFound();
        }

        profile.Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        return View(profile);
    }
}
