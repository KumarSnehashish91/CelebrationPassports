using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Gifting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

// "Gift a Passport" (feature-backlog-v1.1.md, PLAN #14) — v1 uses a mock payment (no
// real gateway). No class-level [Authorize] — Claim's GET is the one page an
// unauthenticated recipient needs to see before logging in, same pattern as
// GuestbookController/ChapterInvitationsController's public entry points.
public class GiftController : Controller
{
    private readonly IPassportGiftService _giftService;

    public GiftController(IPassportGiftService giftService)
    {
        _giftService = giftService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var gifts = await _giftService.GetMyPurchasesAsync();

        foreach (var gift in gifts)
        {
            gift.ClaimUrl = BuildClaimUrl(gift.RedemptionCode);
        }

        ViewData["JustPurchasedClaimUrl"] = TempData["JustPurchasedClaimUrl"];
        ViewData["Error"] = TempData["Error"];

        return View(gifts);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Purchase(PurchaseGiftViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter a recipient name.";
            return RedirectToAction("Index");
        }

        var gift = await _giftService.PurchaseAsync(model);

        if (gift is null)
        {
            TempData["Error"] = "Couldn't create the gift — please try again.";
            return RedirectToAction("Index");
        }

        TempData["JustPurchasedClaimUrl"] = BuildClaimUrl(gift.RedemptionCode);
        return RedirectToAction("Index");
    }

    // ---------- Public — no login required to view ----------

    [HttpGet]
    public async Task<IActionResult> Claim(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return NotFound();
        }

        var info = await _giftService.GetClaimInfoAsync(code);

        if (info is null)
        {
            return NotFound();
        }

        ViewData["Code"] = code;
        return View(info);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ClaimConfirm(string code)
    {
        var passportId = await _giftService.ClaimAsync(code);

        if (passportId is null)
        {
            return RedirectToAction("Claim", new { code });
        }

        // gift-message-schedule-claim.md — first-run banner shown only on this one
        // visit; ?welcomePassportId is the whole "show once" mechanism (a normal later
        // dashboard visit never carries it).
        return RedirectToAction("Index", "Dashboard", new { welcomePassportId = passportId });
    }

    private string BuildClaimUrl(string code) =>
        Url.Action("Claim", "Gift", new { code }, Request.Scheme) ?? string.Empty;
}
