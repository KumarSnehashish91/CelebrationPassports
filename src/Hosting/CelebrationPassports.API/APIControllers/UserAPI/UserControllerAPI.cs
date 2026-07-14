using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.UserAPI;

[ApiController]
[Route("api/[controller]")]
public class UserControllerAPI : ControllerBase
{
    private readonly IuserProfileService _userProfileService;

    public UserControllerAPI(IuserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }


    [HttpGet("GetUserProfile/{userId:guid}")]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        // Implement user profile retrieval logic here
        var userDetails = await _userProfileService.GetUserProfileAsync(userId);
        return Ok(userDetails);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _userProfileService.GetUserProfileAsync(User.GetUserId());
        return Ok(profile);
    }

    [Authorize]
    [HttpPut("me/home-place")]
    public async Task<IActionResult> SetHomePlace(SetHomePlaceRequest request)
    {
        await _userProfileService.SetHomePlaceAsync(User.GetUserId(), request.PlaceId);
        return NoContent();
    }
}

public class SetHomePlaceRequest
{
    public Guid? PlaceId { get; set; }
}

