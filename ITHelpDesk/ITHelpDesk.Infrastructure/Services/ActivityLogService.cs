using System;
using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly AppDbContext _context;

        public ActivityLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(int userId, string action, string entityType, int entityId)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                CreatedAt = DateTime.Now
            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ActivityLogDto>> GetLogsForEntityAsync(string entityType, int entityId)
        {
            var logs = await _context.ActivityLogs
                .Include(a => a.User)
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();

            return logs.Select(a => new ActivityLogDto
            {
                Id = a.Id,
                Action = a.Action,
                PerformedByName = a.User?.FullName ?? string.Empty,
                CreatedAt = a.CreatedAt
            }).ToList();
        }
    }
}