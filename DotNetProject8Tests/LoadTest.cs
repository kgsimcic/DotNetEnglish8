using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DotNetProject8Tests
{
    public class AppointmentStatusResponse
    {
        public long AppointmentId { get; set; }
        public string? Status { get; set; }

    }

    public class LoadTest
    {
        private static readonly HttpClient client = new();

        private string GenerateUniqueId()
        {
            var randomNumber = new Random().Next(1000, 9999);
            var nowString = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            return $"{nowString}{randomNumber}";
        }

        private async Task PollSSE(ConcurrentBag<string> bag, string appointmentId)
        {
            var sseUrl = $"http://localhost:5001/api/sse/{appointmentId}";

            var responseStream = await client.GetStreamAsync(sseUrl);
            using var reader = new StreamReader(responseStream);
            string line; 
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("data: "))
                {
                    var json = line.Substring("data: ".Length);
                    var appointmentStatus = JsonConvert.DeserializeObject<AppointmentStatusResponse>(json);
                    bag.Add(appointmentStatus.Status);
                    break;
                } 
            }
        }

        private async Task PostRequest(ConcurrentBag<bool> bag, string appointmentId)
        {
            var payload = new
            {
                consultant = new
                {
                    consultantId = 1,
                    consultantFname = "Jessica Wally",
                    consultantSpeciality = "Cardiologist"
                },
                appointment = new
                {
                    appointmentId,
                    appointmentDate = "2025-03-22T16:00:00",
                    selectedAppointmentTime = "2025-03-22T16:00:00"
                },
                patient = new
                {
                    patientFName = "P",
                    patientLName = "Slickback",
                    addressLine1 = "whatever",
                    city = "Canada City",
                    postcode = "15111",
                    contactNumber = "7777777777"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"http://localhost:5207/Appointments", content);
            bag.Add(response.IsSuccessStatusCode);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"POST response for {appointmentId} returned {response.StatusCode}.");
            }
        }

        [Fact]
        public async Task SimulateConcurrentClients()
        {
            int clientCount = 200;
            Console.WriteLine("Testing 3000 concurrent requests with the same appointment; so it should have 2999 double bookings.");

            ConcurrentBag<bool> httpBag = [];
            ConcurrentBag<string> sseBag = [];

            var tasks = new List<Task>(); 
            for (int i = 0; i < clientCount; i++)
            {
                var appointmentId = GenerateUniqueId();
                var sseTask = PollSSE(sseBag, appointmentId); 
                tasks.Add(sseTask); 
                var postTask = PostRequest(httpBag, appointmentId); 
                tasks.Add(postTask); 
            }

            await Task.WhenAll(tasks);
            int httpSuccessCount = httpBag.Count(s => s == true);
            int sseCompleteCount = sseBag.Count(s => s == "Completed");
            Console.WriteLine($"{httpSuccessCount} HTTP requests out of {httpBag.Count} returned 200 OK.");
            Console.WriteLine($"{sseCompleteCount} SSE appointments out of {sseBag.Count} total successfully made.");
            Assert.Equal(httpSuccessCount, clientCount);
            Assert.Equal(1, sseCompleteCount);
        }
    }
}
