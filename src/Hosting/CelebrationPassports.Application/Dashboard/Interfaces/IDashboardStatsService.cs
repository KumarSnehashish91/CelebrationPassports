using CelebrationPassports.Application.Dashboard.DTOs;

namespace CelebrationPassports.Application.Dashboard.Interfaces;

public interface IDashboardStatsService
{
    Task<DashboardStatsDto> GetSummaryAsync(Guid userId);
}
