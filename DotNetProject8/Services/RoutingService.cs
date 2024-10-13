namespace DotNetProject8.Services
{
    public class RoutingService : IRoutingService
    {
        private readonly HttpClient _httpClient;

        public RoutingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetConsultantsAsync()
        {
            var response = await _httpClient.GetAsync("http://localhost:5000/gateway/consultants");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


    }
}
