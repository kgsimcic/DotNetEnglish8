using DotNetProject8.Models;
using Newtonsoft.Json;

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

        public async Task<ConsultantCalendarModel> GetConsultantCalendar(int consultantId, int selectedMonth)
        {
            var response = await _httpClient.GetAsync($"/gateway/consultants/{consultantId}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            ConsultantCalendarModel consultantCalendarModel = JsonConvert.DeserializeObject<ConsultantCalendarModel>(responseJson);
            return (consultantCalendarModel);
        }

        public async Task<List<AppointmentDetails>> GetAppointments(int consultantId, int selectedMonth)
        {
            var response = await _httpClient.GetAsync($"/gateway/bookings/{consultantId}-{selectedMonth}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<AppointmentDetails> consultantCalendarModel = JsonConvert.DeserializeObject<List<AppointmentDetails>>(responseJson);
            return (consultantCalendarModel);
        }

        public async Task<int> CreateAppointment(BookingModel bookingModel)
        {
            var response = await _httpClient.PostAsJsonAsync("/gateway/bookings", bookingModel);
            // response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            int result = JsonConvert.DeserializeObject<int>(responseString);
            return (result);
        }
    }
}
