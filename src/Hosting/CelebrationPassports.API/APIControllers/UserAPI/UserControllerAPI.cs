using CelebrationPassports.Application.Users.Interfaces;
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
    //[HttpPut("")]
}

