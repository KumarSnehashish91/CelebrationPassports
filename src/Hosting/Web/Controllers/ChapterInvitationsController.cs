using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class ChapterInvitationsController : Controller
{
    private readonly IChapterSharingService _sharingService;

    public ChapterInvitationsController(IChapterSharingService sharingService)
    {
        _sharingService = sharingService;
    }

    [HttpPost]
    public async Task<IActionResult> Accept(Guid id)
    {
        var chapterId = await _sharingService.AcceptAsync(id);

        return chapterId.HasValue
            ? RedirectToAction("Chapter", "Stories", new { id = chapterId })
            : RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Decline(Guid id)
    {
        await _sharingService.DeclineAsync(id);
        return RedirectToAction("Index", "Dashboard");
    }
}
