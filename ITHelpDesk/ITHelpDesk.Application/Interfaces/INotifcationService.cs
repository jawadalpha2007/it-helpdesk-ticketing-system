using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITHelpDesk.Application.DTOs;

namespace ITHelpDesk.Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string message, int? ticketId);
        Task<List<NotificationDto>> GetNotificationsForUserAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
    }
}
