using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class InvitationsController : Controller
{
    private readonly IInvitationService _invitationService;

    public InvitationsController(IInvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    [HttpPost]
    public async Task<IActionResult> Accept(Guid id)
    {
        await _invitationService.AcceptAsync(id);
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Decline(Guid id)
    {
        await _invitationService.DeclineAsync(id);
        return RedirectToAction("Index", "Dashboard");
    }
}
