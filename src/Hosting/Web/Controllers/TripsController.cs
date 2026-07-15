using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class TripsController : Controller
{
    // Mirrors CelebrationPassports.Web.Models.Events.EventTypeOption's Vacation entry
    // (Value=4) — a "Trip" is a Vacation-type Event, so this reuses the Events feature's
    // status/schedule/cancel machinery entirely rather than a separate domain concept.
    private const int VacationEventType = 4;

    private readonly IEventService _eventService;

    public TripsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    // Mirrors CelebrationPassports.Persistence.Enums.EventStatus (Draft=1, Upcoming=2,
    // Ongoing=3, Completed=4, Cancelled=5).
    public async Task<IActionResult> Index(string tab = "all")
    {
        var normalizedTab = tab.ToLowerInvariant();
        ViewData["ActiveTab"] = normalizedTab;

        int? status = normalizedTab switch
        {
            "upcoming" => 2,
            "ongoing" => 3,
            "past" => 4,
            "draft" => 1,
            "cancelled" => 5,
            _ => null
        };

        var events = await _eventService.GetAllAsync(status);
        var trips = events.Where(e => e.EventType == VacationEventType).ToList();

        return View(trips);
    }
}
