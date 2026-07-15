using System.ComponentModel.DataAnnotations;
using CelebrationPassports.Web.Models.Invitations;

namespace CelebrationPassports.Web.Models.Events;

// One shared model carried across all 4 wizard steps. Each step's controller action
// loads the current state (GetByIdAsync), overlays just its own step's fields, and
// saves the whole thing back (the API's PUT is a full replace, not a patch).
public class EventWizardViewModel
{
    public Guid? Id { get; set; }

    public Guid PassportId { get; set; }

    // Draft while mid-wizard; only Finish (Step 4) flips this to Upcoming.
    public int Status { get; set; } = 1;

    // Step 1 — Event Details
    [Required(ErrorMessage = "Event title is required.")]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select an event type.")]
    public int EventType { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public Guid? CoverMediaId { get; set; }

    public string? CoverImageUrl { get; set; }

    // Step 2 — Date & Time
    public DateOnly? EventDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsAllDay { get; set; }

    public string? TimeZoneId { get; set; } = "(GMT+05:30) India Standard Time (IST)";

    // Step 3 — Location
    public Guid? PlaceId { get; set; }

    public string? PlaceName { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; } = "India";

    [StringLength(200)]
    public string? LocationNotes { get; set; }

    // Step 4 — Invite Guests
    public List<InvitationViewModel> Guests { get; set; } = [];

    // Used by the read-only Preview screen, not the wizard steps.
    public DateTime CreatedAt { get; set; }

    // Set once the event is completed and its Story is generated — used by the Preview
    // screen to scope Memory Map pins down to just this trip's chapters.
    public Guid? StoryId { get; set; }

    // Sub-schedule items under this event (e.g. "Ceremony at 5pm", "Reception at 7pm") —
    // used by the read-only Preview screen, not the wizard steps.
    public List<CalendarEventViewModel> CalendarEvents { get; set; } = [];
}
