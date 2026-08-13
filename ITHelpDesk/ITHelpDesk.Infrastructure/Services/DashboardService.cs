using System;
using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using ITHelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetStatsAsync(int userId, string role)
        {
            var query = _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .AsQueryable();

            if (role == "Employee")
            {
                query = query.Where(t => t.CreatedBy == userId);
            }
            else if (role == "IT Support Agent")
            {
                query = query.Where(t => t.AssignedTo == userId);
            }
            // Manager and Admin see everything

            var tickets = await query.ToListAsync();

            var stats = new DashboardStatsDto
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status?.StatusName == "Open"),
                InProgressTickets = tickets.Count(t => t.Status?.StatusName == "In Progress"),
                ResolvedTickets = tickets.Count(t => t.Status?.StatusName == "Resolved"),
                ClosedTickets = tickets.Count(t => t.Status?.StatusName == "Closed"),
            };

            var resolvedWithTimes = tickets.Where(t => t.ResolvedAt.HasValue).ToList();
            if (resolvedWithTimes.Any())
            {
                stats.AverageResolutionHours = resolvedWithTimes
                    .Average(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours);
            }

            stats.TicketsByStatus = tickets
                .GroupBy(t => t.Status?.StatusName ?? "Unknown")
                .Select(g => new StatusCountDto { StatusName = g.Key, Count = g.Count() })
                .ToList();

            stats.TicketsByPriority = tickets
                .GroupBy(t => t.Priority?.PriorityName ?? "Unknown")
                .Select(g => new PriorityCountDto { PriorityName = g.Key, Count = g.Count() })
                .ToList();

            stats.TicketsByCategory = tickets
                .GroupBy(t => t.Category?.CategoryName ?? "Unknown")
                .Select(g => new CategoryCountDto { CategoryName = g.Key, Count = g.Count() })
                .ToList();

            return stats;
        }
    }
}