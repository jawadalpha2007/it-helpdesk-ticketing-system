using System;
using ITHelpDesk.Application.DTOs;
namespace ITHelpDesk.Application.Interfaces
{
    public interface IActivityLogService
    {
        Task LogActionAsync(int userId, string action, string entityType, int entityId);
        Task<List<ActivityLogDto>> GetLogsForEntityAsync(string entityType, int entityId);
    }
}
