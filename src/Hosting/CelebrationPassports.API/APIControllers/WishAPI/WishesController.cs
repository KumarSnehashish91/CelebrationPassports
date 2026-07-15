using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Wishes.DTOs;
using CelebrationPassports.Application.Wishes.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.WishAPI;

[ApiController]
[Authorize]
public class WishesController : ControllerBase
{
    private readonly IWishService _wishService;

    public WishesController(IWishService wishService)
    {
        _wishService = wishService;
    }

    [HttpPost("api/chapters/{chapterId:guid}/wishes")]
    public async Task<IActionResult> Create(Guid chapterId, CreateWishRequest request)
    {
        var result = await _wishService.CreateAsync(User.GetUserId(), chapterId, request);
        return Ok(result);
    }

    [HttpGet("api/chapters/{chapterId:guid}/wishes")]
    public async Task<IActionResult> GetByChapter(Guid chapterId)
    {
        var result = await _wishService.GetByChapterAsync(User.GetUserId(), chapterId);
        return Ok(result);
    }

    [HttpDelete("api/wishes/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _wishService.DeleteAsync(User.GetUserId(), id);
        return NoContent();
    }
}
