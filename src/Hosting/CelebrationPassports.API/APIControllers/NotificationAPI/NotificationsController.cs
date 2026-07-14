using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.NotificationAPI;

[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("api/notifications/mine")]
    public async Task<IActionResult> Mine([FromQuery] int take = 20)
    {
        var result = await _notificationService.ListMineAsync(User.GetUserId(), take);
        return Ok(result);
    }

    [HttpGet("api/notifications/mine/unread-count")]
    public async Task<IActionResult> UnreadCount()
    {
        var count = await _notificationService.CountUnreadAsync(User.GetUserId());
        return Ok(new { count });
    }

    [HttpPost("api/notifications/{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await _notificationService.MarkAsReadAsync(User.GetUserId(), id);
        return NoContent();
    }
}
