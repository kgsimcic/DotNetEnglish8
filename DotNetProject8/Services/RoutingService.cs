using DotNetProject8.Models;
using DotNetProject8.ViewModels;
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

        public async Task<List<ConsultantViewModel>?> GetConsultantsAsync()
        {
            var response = await _httpClient.GetAsync("/gateway/consultants");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<ConsultantViewModel>? consultantModels = JsonConvert.DeserializeObject<List<ConsultantViewModel>>(responseJson);

            return (consultantModels);
        }

        public async Task<List<ConsultantCalendarViewModel>?> GetConsultantCalendars(int selectedMonth)
        {
            var response = await _httpClient.GetAsync($"/gateway/consultants/{selectedMonth}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<ConsultantCalendarViewModel>? consultantCalendarModels = JsonConvert.DeserializeObject<List<ConsultantCalendarViewModel>?>(responseJson);

            return (consultantCalendarModels);
        }

        public async Task<List<AppointmentModel>> GetAppointments(int consultantId, DateTime selectedDate)
        {
            var dateString = selectedDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var response = await _httpClient.GetAsync($"/gateway/bookings/{dateString}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<AppointmentModel> appointments = JsonConvert.DeserializeObject<List<AppointmentModel>>(responseJson);

            return (appointments.FindAll(a => a.ConsultantId == consultantId));
        }
    }
}
