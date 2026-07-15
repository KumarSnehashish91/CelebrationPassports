using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Calendar.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.CalendarAPI;

[ApiController]
public class CalendarController : ControllerBase
{
    private readonly ICalendarFeedService _feedService;
    private readonly ICalendarFeedTokenService _tokenService;

    public CalendarController(ICalendarFeedService feedService, ICalendarFeedTokenService tokenService)
    {
        _feedService = feedService;
        _tokenService = tokenService;
    }

    // Returns the subscribable feed's path (not the token alone) so the caller doesn't
    // need to know the URL shape — just the API host to prefix it with.
    [Authorize]
    [HttpGet("api/users/me/calendar-feed")]
    public IActionResult GetMyFeedInfo()
    {
        var userId = User.GetUserId();
        var token = _tokenService.GenerateToken(userId);

        return Ok(new { path = $"/api/calendar/{userId}/{token}.ics" });
    }

    // No [Authorize] — calendar apps poll this URL directly and can't send a Bearer
    // token, so the token embedded in the URL itself is the auth (see
    // ICalendarFeedTokenService).
    [HttpGet("api/calendar/{userId:guid}/{token}.ics")]
    public async Task<IActionResult> GetFeed(Guid userId, string token)
    {
        if (!_tokenService.ValidateToken(userId, token))
        {
            return Unauthorized();
        }

        var ics = await _feedService.GenerateIcsAsync(userId);

        return Content(ics, "text/calendar; charset=utf-8");
    }
}
