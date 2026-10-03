using KlayFix.Application.DTOs;

namespace KlayFix.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}
