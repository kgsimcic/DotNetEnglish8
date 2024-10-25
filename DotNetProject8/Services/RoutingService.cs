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
        }

        public async Task<List<ConsultantModel>?> GetConsultantsAsync()
        {
            var response = await _httpClient.GetAsync("http://localhost:5001/gateway/consultants");
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            string responseJson = responseString.Replace("\\", "").Trim(new[] { '"' });

            List<ConsultantModel>? consultantModels = JsonConvert.DeserializeObject<List<ConsultantModel>>(responseJson);
            return (consultantModels);
        }


    }
}
