using System.Security.Claims;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Events;
using CelebrationPassports.Web.Models.Places;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

// Each step accepts its own explicit set of form fields rather than binding the shared
// EventWizardViewModel wholesale — the model carries [Required] fields (Title, EventType)
// that only Step 1's form actually submits, so whole-model binding on Steps 2-4 would
// wipe/fail-validate on fields those forms never send. Every step: load the current
// event fresh, overlay just its own fields, save the merged whole (the API's PUT is a
// full replace, not a patch).
[Authorize]
public class EventsController : Controller
{
    private readonly IEventService _eventService;
    private readonly IPlaceService _placeService;
    private readonly IMediaService _mediaService;
    private readonly IInvitationService _invitationService;
    private readonly IPassportService _passportService;
    private readonly IAIService _aiService;
    private readonly IExpenseService _expenseService;
    private readonly ITripItineraryService _itineraryService;
    private readonly ITripPlannerService _tripPlannerService;
    private readonly IMemoryMapService _memoryMapService;
    private readonly IStoryService _storyService;

    // Mirrors CelebrationPassports.Persistence.Enums.EventStatus.Ongoing — the API
    // computes Upcoming/Ongoing/Completed live from the event's schedule (see
    // EventStatusCalculator) and rejects edits to an Ongoing event with a 409; this
    // check just keeps the user from ever reaching an edit form for one in the first
    // place, rather than letting them fill it out and then hit a save error.
    private const int OngoingStatus = 3;

    // Mirrors CelebrationPassports.Persistence.Enums.EventStatus.Cancelled — same reason
    // as OngoingStatus above: a cancelled event has nothing left to edit.
    private const int CancelledStatus = 5;

    // Mirrors CelebrationPassports.Web.Models.Events.EventTypeOption's Vacation entry
    // (Value=4) — Itinerary and Memory Map on the Preview page only make sense for a
    // trip, same "Trip" == Vacation-type Event mapping used by TripPlannerController.
    private const int VacationEventType = 4;

    public EventsController(
        IEventService eventService,
        IPlaceService placeService,
        IMediaService mediaService,
        IInvitationService invitationService,
        IPassportService passportService,
        IAIService aiService,
        IExpenseService expenseService,
        ITripItineraryService itineraryService,
        ITripPlannerService tripPlannerService,
        IMemoryMapService memoryMapService,
        IStoryService storyService)
    {
        _eventService = eventService;
        _placeService = placeService;
        _mediaService = mediaService;
        _invitationService = invitationService;
        _passportService = passportService;
        _aiService = aiService;
        _expenseService = expenseService;
        _itineraryService = itineraryService;
        _tripPlannerService = tripPlannerService;
        _memoryMapService = memoryMapService;
        _storyService = storyService;
    }

    [HttpGet]
    public async Task<IActionResult> GenerateDescription(string title, int eventType)
    {
        var typeLabel = EventTypeOption.All.FirstOrDefault(o => o.Value == eventType)?.Label ?? "celebration";

        var prompt = string.IsNullOrWhiteSpace(title)
            ? $"Write a short, warm 2-sentence description (under 40 words) for a {typeLabel} event. No preamble, just the description."
            : $"Write a short, warm 2-sentence description (under 40 words) for a {typeLabel} event titled '{title}'. No preamble, just the description.";

        var description = await _aiService.GenerateAsync(prompt);

        return Json(new { description = description?.Trim() });
    }

    // ---------- Step 1: Event Details ----------

    [HttpGet]
    public async Task<IActionResult> Create(Guid? id)
    {
        if (id.HasValue)
        {
            var existing = await _eventService.GetByIdAsync(id.Value);

            if (existing is null)
            {
                return RedirectToAction("Index", "Celebrations");
            }

            if (existing.Status is OngoingStatus or CancelledStatus)
            {
                return RedirectToAction("Preview", new { id });
            }

            existing.Guests = await _invitationService.GetByPassportAsync(existing.PassportId);
            return View(existing);
        }

        var passports = await _passportService.GetMineAsync();
        var passportId = passports.FirstOrDefault()?.Id;

        if (passportId is null)
        {
            return RedirectToAction("Create", "Passports");
        }

        return View(new EventWizardViewModel { PassportId = passportId.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid? id,
        Guid passportId,
        string title,
        int eventType,
        string? description,
        IFormFile? coverImage,
        string action = "next")
    {
        var model = id.HasValue
            ? await _eventService.GetByIdAsync(id.Value) ?? new EventWizardViewModel { PassportId = passportId }
            : new EventWizardViewModel { PassportId = passportId };

        model.Guests = await _invitationService.GetByPassportAsync(model.PassportId);

        model.Title = title;
        model.EventType = eventType;
        model.Description = description;

        if (string.IsNullOrWhiteSpace(title))
        {
            ModelState.AddModelError("title", "Event title is required.");
        }

        if (eventType < 1)
        {
            ModelState.AddModelError("eventType", "Please select an event type.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (coverImage is { Length: > 0 })
        {
            var mediaId = await _mediaService.UploadAsync(coverImage);

            if (mediaId.HasValue)
            {
                model.CoverMediaId = mediaId;
            }
        }

        var savedId = await _eventService.SaveAsync(model);

        if (savedId is null)
        {
            ModelState.AddModelError("", "Could not save the event. Please try again.");
            return View(model);
        }

        if (action == "draft")
        {
            return RedirectToAction("Index", "Celebrations");
        }

        return RedirectToAction("DateTime", new { id = savedId });
    }

    // ---------- Step 2: Date & Time ----------

    [HttpGet]
    public async Task<IActionResult> DateTime(Guid id)
    {
        var model = await _eventService.GetByIdAsync(id);

        if (model is null)
        {
            return RedirectToAction("Index", "Celebrations");
        }

        if (model.Status == OngoingStatus)
        {
            return RedirectToAction("Index", "Celebrations", new { tab = "ongoing" });
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DateTime(
        Guid id,
        DateOnly eventDate,
        TimeOnly? startTime,
        TimeOnly? endTime,
        bool isAllDay,
        string? timeZoneId,
        string action = "next")
    {
        var model = await _eventService.GetByIdAsync(id);

        if (model is null)
        {
            return RedirectToAction("Index", "Celebrations");
        }

        model.EventDate = eventDate;
        model.StartTime = startTime;
        model.EndTime = endTime;
        model.IsAllDay = isAllDay;
        model.TimeZoneId = timeZoneId;

        // Going back saves whatever's already filled in (best-effort, unvalidated) rather
        // than blocking on this step's required fields — the user is leaving, not
        // submitting it.
        if (action == "back")
        {
            await _eventService.SaveAsync(model);
            return RedirectToAction("Create", new { id });
        }

        if (eventDate == default)
        {
            ModelState.AddModelError("eventDate", "Event date is required.");
        }

        if (!isAllDay && startTime is null)
        {
            ModelState.AddModelError("startTime", "Start time is required.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var savedId = await _eventService.SaveAsync(model);

        if (savedId is null)
        {
            ModelState.AddModelError("", "Could not save the event. Please try again.");
            return View(model);
        }

        if (action == "draft")
        {
            return RedirectToAction("Index", "Celebrations");
        }

        return RedirectToAction("Location", new { id });
    }

    // ---------- Step 3: Location ----------

    [HttpGet]
    public async Task<IActionResult> Location(Guid id)
    {
        var model = await _eventService.GetByIdAsync(id);

        if (model is null)
        {
            return RedirectToAction("Index", "Celebrations");
        }

        if (model.Status == OngoingStatus)
        {
            return RedirectToAction("Index", "Celebrations", new { tab = "ongoing" });
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> SearchPlaces(string query)
    {
        var results = await _placeService.SearchAsync(query);
        return Json(results);
    }

    [HttpPost]
    public async Task<IActionResult> Location(
        Guid id,
        Guid? selectedPlaceId,
        string? venueName,
        string? address,
        string? city,
        string? postalCode,
        string? country,
        string? locationNotes,
        string action = "next")
    {
        var model = await _eventService.GetByIdAsync(id);

        if (model is null)
        {
            return RedirectToAction("Index", "Celebrations");
        }

        model.PlaceName = venueName;
        model.Address = address;
        model.City = city;
        model.PostalCode = postalCode;
        model.Country = country;
        model.LocationNotes = locationNotes;

        // Going back saves the location fields as typed but skips place creation/
        // validation — an incomplete "enter manually" form shouldn't block leaving.
        if (action == "back")
        {
            model.PlaceId = selectedPlaceId ?? model.PlaceId;
            await _eventService.SaveAsync(model);
            return RedirectToAction("DateTime", new { id });
        }

        var placeId = selectedPlaceId;

        if (placeId is null && !string.IsNullOrWhiteSpace(venueName))
        {
            var createPlace = new CreatePlaceViewModel
            {
                Name = venueName,
                Address = address,
                City = city ?? string.Empty,
                PostalCode = postalCode,
                Country = country ?? "India",
                Notes = locationNotes
            };

            if (!TryValidateModel(createPlace, nameof(CreatePlaceViewModel)))
            {
                foreach (var error in ModelState[nameof(CreatePlaceViewModel)]?.Errors ?? [])
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(model);
            }

            placeId = await _placeService.CreateAsync(createPlace);

            if (placeId is null)
            {
                ModelState.AddModelError("", "Could not save the location. Please try again.");
                return View(model);
            }
        }

        model.PlaceId = placeId;

        var savedId = await _eventService.SaveAsync(model);

        if (savedId is null)
        {
            ModelState.AddModelError("", "Could not save the event. Please try again.");
            return View(model);
        }

        if (action == "draft")
        {
            return RedirectToAction("Index", "Celebrations");
        }

        return RedirectToAction("Guests", new { id });
    }

    // ---------- Step 4: Invite Guests ----------

    [HttpGet]
    public async Task<IActionResult> Guests(Guid id)
    {
        var model = await _eventService.GetByIdAsync(id);

        if (model is null)
        {
            return RedirectToAction("Index", "Celebrations");
        }

        if (model.Status == OngoingStatus)
        {
            return RedirectToAction("Index", "Celebrations", new { tab = "ongoing" });
        }

        model.Guests = await _invitationService.GetByPassportAsync(model.PassportId);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> InviteGuest(Guid id, Guid passportId, string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            await _invitationService.InviteAsync(passportId, email);
        }

        return RedirectToAction("Guests", new { id });
    }

    // ---------- Read-only preview (Ongoing events — editing is blocked) ----------

    [HttpGet]
    public async Task<IActionResult> Preview(Guid id)
    {
        var model = await _eventService.GetByIdAsync(id);

        if (model is null)
        {
            return RedirectToAction("Index", "Celebrations");
        }

        model.Guests = await _invitationService.GetByPassportAsync(model.PassportId);

        ViewData["Budget"] = await _expenseService.GetBudgetSummaryAsync(id);

        var expenses = await _expenseService.GetExpensesAsync(id);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(currentUserId, out var userId))
        {
            foreach (var expense in expenses)
            {
                expense.IsMine = expense.CreatedByUserId == userId;
            }
        }

        ViewData["Expenses"] = expenses;

        if (model.EventType == VacationEventType)
        {
            ViewData["Itinerary"] = await _itineraryService.GetByEventAsync(id);

            if (model.StoryId.HasValue)
            {
                var allPins = await _memoryMapService.GetByPassportAsync(model.PassportId);

                // Pins are per-Chapter, and a Chapter only ties back to a trip through
                // its linked Story.
                ViewData["TripPins"] = allPins.Where(p => p.StoryId == model.StoryId).ToList();

                var story = await _storyService.GetByIdAsync(model.StoryId.Value);

                if (story is not null)
                {
                    var photos = new List<string>();

                    // Chapter summaries don't carry full media, so this pulls each
                    // chapter's photos individually — the same N+1-but-small-lists
                    // tradeoff already used for place/cover-image resolution elsewhere,
                    // fine at trip scale (a handful of chapters).
                    foreach (var chapterSummary in story.Chapters)
                    {
                        var chapter = await _storyService.GetChapterByIdAsync(chapterSummary.Id);

                        if (chapter is not null)
                        {
                            photos.AddRange(chapter.Media.Where(m => m.Type == 1).Select(m => m.Url));
                        }
                    }

                    ViewData["TripPhotoCount"] = photos.Count;
                    ViewData["TripPhotos"] = photos.Take(8).ToList();
                }
            }
            else
            {
                // No Story linked yet — offer to link one instead of silently showing
                // an empty map, so there's an actual next step to get real pins.
                var stories = await _storyService.GetMineAsync();
                ViewData["LinkableStories"] = stories.Where(s => s.PassportId == model.PassportId).ToList();
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LinkStory(Guid id, Guid storyId)
    {
        await _eventService.LinkStoryAsync(id, storyId);
        return RedirectToAction("Preview", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> GenerateItinerary(Guid id, string destination, int days, string? notes)
    {
        var plan = await _tripPlannerService.GenerateAsync(destination, days, notes);

        if (plan is not null && plan.Itinerary.Count > 0)
        {
            await _itineraryService.SaveItineraryAsync(id, plan.Itinerary);
        }
        else
        {
            TempData["ItineraryError"] = "Could not generate an itinerary right now — the local AI model may be unavailable. Please try again.";
        }

        return RedirectToAction("Preview", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> AddExpense(Guid id, int category, string? description, decimal amount, DateOnly? spentOn)
    {
        if (amount > 0)
        {
            await _expenseService.AddExpenseAsync(id, category, description, amount, spentOn);
        }

        return RedirectToAction("Preview", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteExpense(Guid id, Guid expenseId)
    {
        await _expenseService.DeleteExpenseAsync(expenseId);
        return RedirectToAction("Preview", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> SetCategoryBudget(Guid id, int category, decimal budgetedAmount)
    {
        if (budgetedAmount >= 0)
        {
            await _expenseService.SetCategoryBudgetAsync(id, category, budgetedAmount);
        }

        return RedirectToAction("Preview", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Finish(Guid id, string action = "finish")
    {
        if (action == "back")
        {
            return RedirectToAction("Location", new { id });
        }

        if (action != "draft")
        {
            // Publishes as Upcoming — whether it's actually Upcoming/Ongoing/Completed
            // from here on is computed live from the event's own schedule, not chosen.
            await _eventService.FinalizeAsync(id);
        }

        return RedirectToAction("Index", "Celebrations");
    }

    [HttpPost]
    public async Task<IActionResult> AddCalendarEvent(Guid id, string title, string? location, DateTime eventTime, string colorTag)
    {
        await _eventService.AddCalendarEventAsync(id, new CalendarEventViewModel
        {
            Title = title,
            Location = location,
            EventTime = eventTime,
            ColorTag = colorTag
        });

        return RedirectToAction("Preview", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _eventService.CancelAsync(id);
        return RedirectToAction("Index", "Celebrations");
    }
}
