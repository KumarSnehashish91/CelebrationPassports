using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Gifting.DTOs;
using CelebrationPassports.Application.Gifting.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.GiftingAPI;

// No class-level [Authorize] — GetClaimInfo is the one public, unauthenticated entry
// point (whoever opens the claim link, logged in or not), same pattern as
// GuestbookController.
[ApiController]
[Route("api/gifts")]
public class PassportGiftsController : ControllerBase
{
    private readonly IPassportGiftService _giftService;

    public PassportGiftsController(IPassportGiftService giftService)
    {
        _giftService = giftService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Purchase(PurchaseGiftRequest request)
    {
        var result = await _giftService.PurchaseAsync(User.GetUserId(), request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var result = await _giftService.GetMyPurchasesAsync(User.GetUserId());
        return Ok(result);
    }

    [Authorize]
    [HttpPost("claim/{code}")]
    public async Task<IActionResult> Claim(string code)
    {
        var passportId = await _giftService.ClaimAsync(User.GetUserId(), code);
        return Ok(new { passportId });
    }

    [Authorize]
    [HttpGet("passports/{passportId:guid}/message")]
    public async Task<IActionResult> GetMessage(Guid passportId)
    {
        var result = await _giftService.GetMessageAsync(User.GetUserId(), passportId);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpGet("passports/{passportId:guid}/claimed-summary")]
    public async Task<IActionResult> GetClaimedSummary(Guid passportId)
    {
        var result = await _giftService.GetClaimedGiftSummaryAsync(User.GetUserId(), passportId);
        return result is null ? NotFound() : Ok(result);
    }

    // ---------- Public — no login, just the redemption code ----------

    [HttpGet("claim/{code}")]
    public async Task<IActionResult> GetClaimInfo(string code)
    {
        var result = await _giftService.GetClaimInfoAsync(code);
        return result is null ? NotFound() : Ok(result);
    }
}
