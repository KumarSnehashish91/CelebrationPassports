using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class CalendarController : Controller
{
    private readonly IEventService _eventService;

    public CalendarController(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _eventService.GetAllAsync(null);
        return View(events);
    }
}
