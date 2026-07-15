using CelebrationPassports.API.Extensions;
using CelebrationPassports.Application.Dashboard.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.API.APIControllers.DashboardAPI;

[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardStatsService _statsService;

    public DashboardController(IDashboardStatsService statsService)
    {
        _statsService = statsService;
    }

    [HttpGet("api/dashboard/stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _statsService.GetSummaryAsync(User.GetUserId());
        return Ok(result);
    }
}
