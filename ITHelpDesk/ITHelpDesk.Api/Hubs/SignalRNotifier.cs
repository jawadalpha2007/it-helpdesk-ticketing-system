using ITHelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ITHelpDesk.Api.Hubs
{
    public class SignalRNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(int userId, object payload)
        {
            await _hubContext.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", payload);
        }
    }
}