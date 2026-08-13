using System;
using ITHelpDesk.Application.DTOs;

namespace ITHelpDesk.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync(int userId, string role);
    }
}