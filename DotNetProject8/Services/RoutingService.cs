using DotNetProject8.Models;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DotNetProject8.Services
{
    public class RoutingService : IRoutingService
    {
        private readonly HttpClient _httpClient;

        public RoutingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5001");
        }

        public async Task<List<ConsultantModel>?> GetConsultantsAsync()
        {
            var response = await _httpClient.GetAsync("/gateway/consultants");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<ConsultantModel>? consultantModels = JsonConvert.DeserializeObject<List<ConsultantModel>>(responseJson);

            return (consultantModels);
        }

        public async Task<List<ConsultantCalendarModel>?> GetConsultantCalendars(int selectedMonth)
        {
            var response = await _httpClient.GetAsync($"/gateway/consultants/{selectedMonth}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<ConsultantCalendarModel>? consultantCalendarModels = JsonConvert.DeserializeObject<List<ConsultantCalendarModel>?>(responseJson);

            return (consultantCalendarModels);
        }

        public async Task<List<AppointmentResponse>> GetAppointments(int consultantId, DateTime selectedDate)
        {
            var dateString = selectedDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var response = await _httpClient.GetAsync($"/gateway/bookings/{dateString}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<AppointmentResponse> appointments = JsonConvert.DeserializeObject<List<AppointmentResponse>>(responseJson);

            return (appointments.FindAll(a => a.ConsultantId == consultantId));
        }
    }
}
