using CelebrationPassports.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public IActionResult Index()
    {
        var model = _dashboardService.GetDashboard();

        return View(model);
    }
}