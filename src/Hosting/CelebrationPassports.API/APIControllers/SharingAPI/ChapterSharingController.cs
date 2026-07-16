using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Sharing.DTOs;
using CelebrationPassports.Application.Sharing.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.SharingAPI;

[ApiController]
[Authorize]
public class ChapterSharingController : ControllerBase
{
    private readonly IChapterSharingService _sharingService;

    public ChapterSharingController(IChapterSharingService sharingService)
    {
        _sharingService = sharingService;
    }

    [HttpPost("api/chapters/{chapterId:guid}/sharing/invite")]
    public async Task<IActionResult> Invite(Guid chapterId, InviteChapterContributorRequest request)
    {
        var result = await _sharingService.InviteAsync(User.GetUserId(), chapterId, request);
        return Ok(result);
    }

    [HttpGet("api/chapters/{chapterId:guid}/sharing/invitations")]
    public async Task<IActionResult> GetInvitations(Guid chapterId)
    {
        var result = await _sharingService.GetByChapterAsync(User.GetUserId(), chapterId);
        return Ok(result);
    }

    [HttpGet("api/chapters/{chapterId:guid}/sharing/contributors")]
    public async Task<IActionResult> GetContributors(Guid chapterId)
    {
        var result = await _sharingService.GetContributorsAsync(User.GetUserId(), chapterId);
        return Ok(result);
    }

    [HttpDelete("api/chapters/sharing/contributors/{contributorId:guid}")]
    public async Task<IActionResult> RemoveContributor(Guid contributorId)
    {
        await _sharingService.RemoveContributorAsync(User.GetUserId(), contributorId);
        return NoContent();
    }

    [HttpGet("api/chapter-invitations/mine")]
    public async Task<IActionResult> GetMine()
    {
        var result = await _sharingService.GetPendingForMeAsync(User.GetUserId(), User.GetEmail());
        return Ok(result);
    }

    [HttpPost("api/chapter-invitations/{id:guid}/accept")]
    public async Task<IActionResult> Accept(Guid id)
    {
        var chapterId = await _sharingService.AcceptAsync(User.GetUserId(), id, User.GetEmail());
        return Ok(new { chapterId });
    }

    [HttpPost("api/chapter-invitations/{id:guid}/decline")]
    public async Task<IActionResult> Decline(Guid id)
    {
        await _sharingService.DeclineAsync(User.GetUserId(), id, User.GetEmail());
        return Ok();
    }
}
