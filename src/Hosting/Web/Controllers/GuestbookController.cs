using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

// No class-level [Authorize] — Submit is the one public, unauthenticated entry point a
// guest reaches via their unguessable link; Approve/Reject are member-only actions
// reached from the (authenticated) Chapter page.
public class GuestbookController : Controller
{
    private readonly IGuestbookService _guestbookService;

    public GuestbookController(IGuestbookService guestbookService)
    {
        _guestbookService = guestbookService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Approve(Guid chapterId, Guid submissionId)
    {
        await _guestbookService.ApproveAsync(submissionId);
        return RedirectToAction("Chapter", "Stories", new { id = chapterId });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Reject(Guid chapterId, Guid submissionId)
    {
        await _guestbookService.RejectAsync(submissionId);
        return RedirectToAction("Chapter", "Stories", new { id = chapterId });
    }

    // ---------- Public — no login ----------

    [HttpGet]
    public async Task<IActionResult> Submit(Guid chapterId, string token)
    {
        var info = await _guestbookService.GetPublicInfoAsync(chapterId, token);

        if (info is null)
        {
            ViewData["InvalidLink"] = true;
            return View();
        }

        ViewData["ChapterId"] = chapterId;
        ViewData["Token"] = token;
        return View(info);
    }

    [HttpPost]
    public async Task<IActionResult> Submit(Guid chapterId, string token, string guestName, string? message, IFormFile? photo)
    {
        if (string.IsNullOrWhiteSpace(guestName))
        {
            ViewData["SubmitError"] = "Please enter your name.";
            ViewData["ChapterId"] = chapterId;
            ViewData["Token"] = token;
            return View(await _guestbookService.GetPublicInfoAsync(chapterId, token));
        }

        var (success, error) = await _guestbookService.SubmitAsync(chapterId, token, guestName, message, photo);

        if (!success)
        {
            ViewData["SubmitError"] = error ?? "Could not submit — please try again.";
            ViewData["ChapterId"] = chapterId;
            ViewData["Token"] = token;
            return View(await _guestbookService.GetPublicInfoAsync(chapterId, token));
        }

        return RedirectToAction("Thanks", new { chapterId, token });
    }

    [HttpGet]
    public IActionResult Thanks(Guid chapterId, string token)
    {
        ViewData["ChapterId"] = chapterId;
        ViewData["Token"] = token;
        return View();
    }
}
