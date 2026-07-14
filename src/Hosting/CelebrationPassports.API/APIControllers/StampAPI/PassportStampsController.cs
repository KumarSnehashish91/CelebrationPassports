using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Stamps.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.StampAPI;

[ApiController]
[Authorize]
public class PassportStampsController : ControllerBase
{
    private readonly IPassportStampService _stampService;

    public PassportStampsController(IPassportStampService stampService)
    {
        _stampService = stampService;
    }

    [HttpGet("api/passport-stamps/mine/count")]
    public async Task<IActionResult> GetMyCount()
    {
        var count = await _stampService.GetCountForUserAsync(User.GetUserId());
        return Ok(new { count });
    }
}
