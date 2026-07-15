using System.Security.Claims;
using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Passports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class PassportsController : Controller
{
    private readonly IPassportService _passportService;
    private readonly IInvitationService _invitationService;

    public PassportsController(IPassportService passportService, IInvitationService invitationService)
    {
        _passportService = passportService;
        _invitationService = invitationService;
    }

    public async Task<IActionResult> Index()
    {
        var passports = await _passportService.GetMineAsync();
        return View(passports);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var passport = await _passportService.GetByIdAsync(id);

        if (passport is null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        passport.IsOwner = Guid.TryParse(currentUserId, out var userId) && passport.OwnerUserId == userId;

        if (passport.IsOwner)
        {
            var invitations = await _invitationService.GetByPassportAsync(id);
            ViewData["PendingInvitations"] = invitations.Where(i => i.Status == "Pending").ToList();
        }

        return View(passport);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePassportViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePassportViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _passportService.CreateAsync(model);

        if (!success)
        {
            ModelState.AddModelError("", "Could not create the passport. Please try again.");
            return View(model);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Invite(Guid passportId, string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            var success = await _invitationService.InviteAsync(passportId, email);

            if (!success)
            {
                TempData["InviteError"] = "Could not send that invitation — they may already have a pending invite to this passport.";
            }
        }

        return RedirectToAction("Details", new { id = passportId });
    }
}
