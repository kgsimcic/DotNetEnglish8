using DotNetProject8.Models;
using DotNetProject8.Services;
using Microsoft.AspNetCore.SignalR;

namespace DotNetProject8
{
    public class AppointmentHub : Hub
    {
        private readonly IRoutingService _routingService; 
        public AppointmentHub(IRoutingService routingService) { 
            _routingService = routingService; 
        }

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            return base.OnConnectedAsync();
        }
        //public async Task SendStatusUpdate(AppointmentStatusResponse update)
        //{
        //  await Clients.Client(connectionId).SendAsync("ReceiveStatusUpdate", update); 
        //}
    }
}
