using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Gifting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

// "Gift Photo Story & Print-Ready Preview" (gift-story-print-preview.md) — extends
// Gift a Passport with a four-step wizard: Photos → Insights → Story → Print Preview.
// Finalize hands off into the same PassportGift purchase/redemption-link flow
// GiftController already built for the simple (no-story) gift path.
[Authorize]
public class GiftStoryController : Controller
{
    private readonly IGiftStoryService _giftStoryService;

    public GiftStoryController(IGiftStoryService giftStoryService)
    {
        _giftStoryService = giftStoryService;
    }

    [HttpPost]
    public async Task<IActionResult> Start(PurchaseGiftViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter a recipient name.";
            return RedirectToAction("Index", "Gift");
        }

        var draft = await _giftStoryService.StartAsync(model);

        if (draft is null)
        {
            TempData["Error"] = "Couldn't start the gift story — please try again.";
            return RedirectToAction("Index", "Gift");
        }

        return RedirectToAction("Photos", new { id = draft.Id });
    }

    public async Task<IActionResult> Photos(Guid id, string? error)
    {
        var draft = await _giftStoryService.GetAsync(id);

        if (draft is null)
        {
            return NotFound();
        }

        ViewData["Error"] = error;
        return View(draft);
    }

    [HttpPost]
    [RequestSizeLimit(25 * 1024 * 1024 * 10)]
    public async Task<IActionResult> UploadPhotos(Guid id, List<IFormFile> files)
    {
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                await _giftStoryService.AddPhotoAsync(id, file);
            }
        }

        return RedirectToAction("Photos", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> RemovePhoto(Guid draftId, Guid photoId)
    {
        await _giftStoryService.RemovePhotoAsync(photoId);
        return RedirectToAction("Photos", new { id = draftId });
    }

    [HttpPost]
    public async Task<IActionResult> ContinueToInsights(Guid id)
    {
        var draft = await _giftStoryService.ContinueToInsightsAsync(id);

        if (draft is null)
        {
            return RedirectToAction("Photos", new { id, error = "Add at least 4 photos before continuing." });
        }

        return RedirectToAction("Insights", new { id });
    }

    public async Task<IActionResult> Insights(Guid id)
    {
        var draft = await _giftStoryService.GetAsync(id);

        if (draft is null)
        {
            return NotFound();
        }

        return View(draft);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateStory(Guid id)
    {
        // One "insight_{photoId}" text field per photo — read directly from the form
        // rather than model-binding, since the set of field names is dynamic (one per
        // uploaded photo, not a fixed shape).
        var insights = Request.Form.Keys
            .Where(key => key.StartsWith("insight_", StringComparison.Ordinal) &&
                          Guid.TryParse(key["insight_".Length..], out _))
            .ToDictionary(
                key => Guid.Parse(key["insight_".Length..]),
                key => (string?)Request.Form[key].ToString());

        await _giftStoryService.SetInsightsAsync(id, insights);
        await _giftStoryService.GenerateStoryAsync(id);

        return RedirectToAction("Story", new { id });
    }

    public async Task<IActionResult> Story(Guid id)
    {
        var draft = await _giftStoryService.GetAsync(id);
        var story = await _giftStoryService.GetStoryAsync(id);

        if (draft is null || story is null)
        {
            return RedirectToAction("Insights", new { id });
        }

        ViewData["Draft"] = draft;
        return View(story);
    }

    [HttpPost]
    public async Task<IActionResult> Regenerate(Guid id)
    {
        await _giftStoryService.RegenerateStoryAsync(id);
        return RedirectToAction("Story", new { id });
    }

    public async Task<IActionResult> Print(Guid id, string format = "LifeBook")
    {
        var preview = await _giftStoryService.GetPrintPreviewAsync(id, format);

        if (preview is null)
        {
            return RedirectToAction("Story", new { id });
        }

        ViewData["DraftId"] = id;
        return View(preview);
    }

    public async Task<IActionResult> Message(Guid id, string format)
    {
        var draft = await _giftStoryService.GetAsync(id);

        if (draft is null)
        {
            return NotFound();
        }

        ViewData["Format"] = format;
        return View(draft);
    }

    // Step 5 (gift-message-schedule-claim.md) — captures the message + delivery
    // schedule, then finalizes straight into the existing checkout/redemption-link flow.
    [HttpPost]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> SaveMessageAndFinalize(
        Guid id, string format, string messageType, IFormFile? media, string? writtenText,
        string deliveryMode, DateTime? scheduledDate)
    {
        await _giftStoryService.SetMessageAsync(id, messageType, media, writtenText);
        await _giftStoryService.SetDeliveryScheduleAsync(id, deliveryMode, scheduledDate);

        var gift = await _giftStoryService.FinalizeAsync(id, format);

        if (gift is null)
        {
            TempData["Error"] = "Couldn't finalize this gift — please try again.";
            return RedirectToAction("Message", new { id, format });
        }

        TempData["JustPurchasedClaimUrl"] = Url.Action("Claim", "Gift", new { code = gift.RedemptionCode }, Request.Scheme);
        return RedirectToAction("Index", "Gift");
    }
}
