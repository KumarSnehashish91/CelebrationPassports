using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Open(Guid id)
    {
        var notifications = await _notificationService.GetMineAsync(50);
        var notification = notifications.FirstOrDefault(n => n.Id == id);

        await _notificationService.MarkAsReadAsync(id);

        if (notification is not null && !string.IsNullOrWhiteSpace(notification.ActionUrl))
        {
            return LocalRedirect(notification.ActionUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }
}
