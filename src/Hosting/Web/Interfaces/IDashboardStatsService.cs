using CelebrationPassports.Web.Models.Dashboard;

namespace CelebrationPassports.Web.Interfaces;

public interface IDashboardStatsService
{
    Task<DashboardStatsViewModel> GetSummaryAsync();
}
