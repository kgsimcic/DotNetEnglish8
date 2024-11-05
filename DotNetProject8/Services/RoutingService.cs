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

        public async Task<List<ConsultantCalendarModel>?> GetConsultantCalendars(int selectedMonth)
        {
            var response = await _httpClient.GetAsync($"/gateway/consultants/{selectedMonth}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<ConsultantCalendarModel>? consultantCalendarModels = JsonConvert.DeserializeObject<List<ConsultantCalendarModel>?>(responseJson);

            return (consultantCalendarModels);
        }

        public async Task<List<AppointmentDetails>> GetAppointments(int selectedMonth)
        {
            var response = await _httpClient.GetAsync($"/gateway/bookings/{selectedMonth}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<AppointmentDetails> consultantCalendarModel = JsonConvert.DeserializeObject<List<AppointmentDetails>>(responseJson);

            return (consultantCalendarModel);
        }

        public async Task<string> GetBookingStatus(Guid appointmentId)
        {
            var response = await _httpClient.GetAsync($"/gateway/bookings/status/{appointmentId}");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            // edit later - going to be a string containing status. 
            string bookingStatus = JsonConvert.DeserializeObject<string>(responseJson);
            return (bookingStatus);
        }

        public async Task<Guid> CreateAppointment(BookingModel bookingModel)
        {
            var response = await _httpClient.PostAsJsonAsync("/gateway/bookings", bookingModel);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            Guid appointmentId = JsonConvert.DeserializeObject<Guid>(responseString);
            return (appointmentId);
        }
    }
}
