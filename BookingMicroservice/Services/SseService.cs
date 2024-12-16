using BookingMicroservice.Models;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace BookingMicroservice.Services
{
    public class SseService
    {
        private readonly ILogger<SseService> _logger;
        private readonly ConcurrentDictionary<long, HttpResponse> _clients = new (); 

        public SseService(ILogger<SseService> logger)
        {
            _logger = logger;
        }

        public async Task SubscribeAsync(long appointmentId, HttpResponse response)
        {
            response.Headers.Append("Content-Type", "text/event-stream"); 
            _clients[appointmentId] = response;
            _logger.LogInformation($"SSE Service: {appointmentId} subscribed");
            await Task.Delay(-1, response.HttpContext.RequestAborted);
            _clients.TryRemove(appointmentId, out _); 
        } 
        
        public async Task SendUpdateAsync(long appointmentId, AppointmentStatusResponse appointmentStatusResponse) {
            _logger.LogInformation($"SSE Service: sending {appointmentId} status....");
            if (_clients.TryGetValue(appointmentId, out var response)) { 
                string json = JsonConvert.SerializeObject(appointmentStatusResponse); 

                await response.WriteAsync($"data: {json}\n\n"); 
                await response.Body.FlushAsync(); 
            } 
        }
    }
}
