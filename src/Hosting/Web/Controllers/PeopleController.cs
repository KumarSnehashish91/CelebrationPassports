using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.People;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

[Authorize]
public class PeopleController : Controller
{
    private readonly IPassportService _passportService;

    public PeopleController(IPassportService passportService)
    {
        _passportService = passportService;
    }

    public async Task<IActionResult> Index()
    {
        var passports = await _passportService.GetMineAsync();
        var groups = new List<PassportPeopleGroupViewModel>();

        foreach (var passport in passports)
        {
            var detail = await _passportService.GetByIdAsync(passport.Id);

            if (detail is null)
            {
                continue;
            }

            groups.Add(new PassportPeopleGroupViewModel
            {
                PassportId = passport.Id,
                PassportTitle = passport.Title,
                People = detail.People
            });
        }

        return View(groups);
    }
}
