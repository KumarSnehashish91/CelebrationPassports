using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Invitations.DTOs;
using CelebrationPassports.Application.Invitations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.InvitationAPI;

[ApiController]
[Authorize]
public class InvitationsController : ControllerBase
{
    private readonly IPassportInvitationService _invitationService;

    public InvitationsController(IPassportInvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    [HttpPost("api/passports/{passportId:guid}/invitations")]
    public async Task<IActionResult> Invite(Guid passportId, CreateInvitationRequest request)
    {
        var result = await _invitationService.InviteAsync(User.GetUserId(), passportId, request);
        return Ok(result);
    }

    [HttpGet("api/passports/{passportId:guid}/invitations")]
    public async Task<IActionResult> GetByPassport(Guid passportId)
    {
        var result = await _invitationService.GetByPassportAsync(User.GetUserId(), passportId);
        return Ok(result);
    }

    [HttpGet("api/invitations/mine")]
    public async Task<IActionResult> GetMine()
    {
        var result = await _invitationService.GetPendingForMeAsync(User.GetUserId(), User.GetEmail());
        return Ok(result);
    }

    [HttpPost("api/invitations/{id:guid}/accept")]
    public async Task<IActionResult> Accept(Guid id)
    {
        await _invitationService.AcceptAsync(User.GetUserId(), id, User.GetEmail());
        return Ok();
    }

    [HttpPost("api/invitations/{id:guid}/decline")]
    public async Task<IActionResult> Decline(Guid id)
    {
        await _invitationService.DeclineAsync(User.GetUserId(), id, User.GetEmail());
        return Ok();
    }
}
