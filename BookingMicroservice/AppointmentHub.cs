using Microsoft.AspNetCore.SignalR;

namespace BookingMicroservice
{
    public class AppointmentHub : Hub
    {
        private readonly ILogger<AppointmentHub> _logger;

        public AppointmentHub(ILogger<AppointmentHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var sessionId = httpContext?.Request.Query["sessionId"].ToString();
                if (!string.IsNullOrEmpty(sessionId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
                    _logger.LogInformation($"Client added to group {sessionId}");
                }
                else
                {
                    _logger.LogInformation($"No session Id for this connection.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var sessionId = httpContext?.Request.Query["sessionId"].ToString();
                if (!string.IsNullOrEmpty(sessionId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
                    _logger.LogInformation($"Client removed from group {sessionId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
