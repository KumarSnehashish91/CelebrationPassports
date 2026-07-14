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
        // Ongoing=3, Completed=4). "Cancelled" has no backing status yet, so that tab is
        // rendered but always yields an empty list rather than fabricating data.
        public async Task<IActionResult> Index(string tab = "all")
        {
            var normalizedTab = tab.ToLowerInvariant();
            ViewData["ActiveTab"] = normalizedTab;

            // "Cancelled" has no backing status yet — short-circuit rather than
            // sending an undefined enum value to the API.
            if (normalizedTab == "cancelled")
            {
                return View(new List<CelebrationListItemViewModel>());
            }

            int? status = normalizedTab switch
            {
                "upcoming" => 2,
                "ongoing" => 3,
                "past" => 4,
                "draft" => 1,
                _ => null
            };

            var events = await _eventService.GetAllAsync(status);
            return View(events);
        }
    }
}
