using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BookingMicroservice.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IHubContext<AppointmentHub> _hubContext;

        public NotificationService(ILogger<NotificationService> logger, IHubContext<AppointmentHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }


        public async Task NotifyBookingStatus(string sessionId, string status)
        {
            try
            {
                await _hubContext.Clients.Group(sessionId)
                    .SendAsync("ReceiveStatusUpdate", status);
                _logger.LogInformation($"Notification sent to group {sessionId}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to notify group {sessionId}");
            }
        }
    }
}
