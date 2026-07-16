using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Guestbook.DTOs;
using CelebrationPassports.Application.Guestbook.Interfaces;
using CelebrationPassports.Application.Media.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.GuestbookAPI;

// No class-level [Authorize] — this controller deliberately mixes authenticated
// (member-only) actions with the two anonymous ones a guest needs (GetPublicInfo,
// Submit), each marked individually below.
[ApiController]
public class GuestbookController : ControllerBase
{
    private readonly IGuestbookService _guestbookService;

    public GuestbookController(IGuestbookService guestbookService)
    {
        _guestbookService = guestbookService;
    }

    [Authorize]
    [HttpGet("api/chapters/{chapterId:guid}/guestbook/link")]
    public async Task<IActionResult> GetLink(Guid chapterId)
    {
        var token = await _guestbookService.GetShareTokenAsync(User.GetUserId(), chapterId);
        return Ok(new { token });
    }

    [Authorize]
    [HttpGet("api/chapters/{chapterId:guid}/guestbook/pending")]
    public async Task<IActionResult> GetPending(Guid chapterId)
    {
        var result = await _guestbookService.GetPendingAsync(User.GetUserId(), chapterId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("api/guestbook/{submissionId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid submissionId)
    {
        await _guestbookService.ApproveAsync(User.GetUserId(), submissionId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("api/guestbook/{submissionId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid submissionId)
    {
        await _guestbookService.RejectAsync(User.GetUserId(), submissionId);
        return NoContent();
    }

    // ---------- Anonymous — no login, just an unguessable token ----------

    [HttpGet("api/guestbook/{chapterId:guid}/info")]
    public async Task<IActionResult> GetPublicInfo(Guid chapterId, [FromQuery] string token)
    {
        var result = await _guestbookService.GetPublicInfoAsync(chapterId, token);
        return Ok(result);
    }

    [HttpPost("api/guestbook/{chapterId:guid}/submit")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Submit(Guid chapterId, [FromForm] SubmitGuestbookFormModel form)
    {
        FileUploadRequest? photo = null;
        Stream? stream = null;

        try
        {
            if (form.Photo is { Length: > 0 })
            {
                stream = form.Photo.OpenReadStream();
                photo = new FileUploadRequest
                {
                    Content = stream,
                    FileName = form.Photo.FileName,
                    Length = form.Photo.Length
                };
            }

            await _guestbookService.SubmitAsync(
                chapterId,
                form.Token,
                new SubmitGuestbookEntryRequest { GuestName = form.GuestName, Message = form.Message },
                photo);
        }
        finally
        {
            if (stream is not null)
            {
                await stream.DisposeAsync();
            }
        }

        return NoContent();
    }
}

public class SubmitGuestbookFormModel
{
    public string Token { get; set; } = string.Empty;

    public string GuestName { get; set; } = string.Empty;

    public string? Message { get; set; }

    public IFormFile? Photo { get; set; }
}
