using Microsoft.AspNetCore.SignalR.Client;

namespace DotNetProject8.Services
{
    public class SignalRService
    {
        private readonly HubConnection _connection;

        public SignalRService() { 
            _connection = new HubConnectionBuilder().WithUrl("http://localhost:8081/appointmentHub").Build();
            _connection.StartAsync().Wait();
        }

        public async Task<string> GetConnectionId()
        {
            await Task.Yield();
            return _connection.ConnectionId; 
        }

        public void OnReceiveUpdate(Action<string, string> callback) { 
            _connection.On("ReceiveUpdate", callback); 
        }
    }
}
