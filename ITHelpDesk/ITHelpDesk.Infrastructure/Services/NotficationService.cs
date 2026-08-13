using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public NotificationService(AppDbContext context, IRealtimeNotifier realtimeNotifier)
        {
            _context = context;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task CreateNotificationAsync(int userId, string message, int? ticketId)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                TicketId = ticketId,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _realtimeNotifier.SendToUserAsync(userId, new
            {
                id = notification.Id,
                message = notification.Message,
                ticketId = notification.TicketId,
                createdAt = notification.CreatedAt
            });
        }

        public async Task<List<NotificationDto>> GetNotificationsForUserAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(30)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                TicketId = n.TicketId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}