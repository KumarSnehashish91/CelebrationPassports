using CelebrationPassports.Web.ViewModels.Dashboard;

namespace CelebrationPassports.Web.Interfaces;

public interface IDashboardService
{
    DashboardViewModel GetDashboard();
}