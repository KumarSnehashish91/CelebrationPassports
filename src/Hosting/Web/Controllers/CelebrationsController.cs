using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers
{
    [Authorize]
    public class CelebrationsController : Controller
    {
        private readonly IEventService _eventService;

        public CelebrationsController(IEventService eventService)
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
            return View(events);
        }
    }
}
