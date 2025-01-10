using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotNetProject8Tests
{

    public class LoadTest
    {
        private readonly HttpClient _httpClient;
        ConcurrentBag<TestResult> _results = new();
        private const int REQUEST_TIMEOUT_SECONDS = 30;

        public LoadTest()
        {
            ServicePointManager.DefaultConnectionLimit = 3000;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            var services = new ServiceCollection();

            services.AddHttpClient("client", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5207");
                client.Timeout = TimeSpan.FromSeconds(300);
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 3500,
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(5)
            });

            var serviceProvider = services.BuildServiceProvider();
            _httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("client");
        }

        public class TestResult
        {
            public int RequestId { get; set; }
            public bool RequestSuccess { get; set; }
            public DateTime? HttpRequestDt { get; set; }
            public TimeSpan Duration { get; set; }
            public string Status { get; set; } = string.Empty;
            public Exception? Error { get; set; }
            public string? TimeoutType { get; set; }
            public bool SignalRReceived { get; set; }
        }

        public void PrintResults(TimeSpan totalDuration)
        {
            var successCount = _results.Count(r => r.RequestSuccess);
            var successBookingCount = _results.Count(r => r.Status == "Completed");
            var avgDuration = _results.Average(r => r.Duration.TotalSeconds);

            var timeoutsByType = _results
                .Where(r => r.TimeoutType != null)
                .GroupBy(r => r.TimeoutType)
                .ToDictionary(g => g.Key!, g => g.Count());

            Console.WriteLine("\n=== Test Results ===");
            Console.WriteLine($"Successful requests: {successCount} out of {_results.Count}");
            Console.WriteLine($"Successful bookings: {successBookingCount} out of {successCount}");
            Console.WriteLine($"Average duration: {avgDuration:F2} seconds");
            Console.WriteLine($"Total Test Duration: {totalDuration.TotalSeconds:F2} seconds");

            Console.WriteLine("\nTimeout Analysis:");
            foreach (var (type, count) in timeoutsByType)
            {
                Console.WriteLine($"- {type} timeouts: {count}");
            }
        }

        private static object CreateRequestData(string sessionId) => new
        {
            consultant = new
            {
                consultantId = 1,
                consultantFname = "Jessica Wally",
                consultantSpeciality = "Cardiologist"
            },
            appointment = new
            {
                connectionId = sessionId,
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

        private HubConnection SetupHubConnection(string sessionId)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:8081/appointmentHub?sessionId={sessionId}")
                .WithAutomaticReconnect()
                .Build();
            return hubConnection;
        }

        private async Task SimulateBookingRequest(string sessionId, int requestId)
        {
            var startTime = DateTime.Now;
            var result = new TestResult();

            await using var hubConnection = SetupHubConnection(sessionId);
            try
            {
                // SignalR
                var tcs = new TaskCompletionSource<(string status, DateTime endTime)>();

                // actually listen for it
                hubConnection.On<string>("ReceiveStatusUpdate", (status) => {
                    var endTime = DateTime.Now;
                    tcs.TrySetResult((status, endTime));
                });

                hubConnection.Closed += (error) =>
                {
                    if (error != null)
                    {
                        Console.WriteLine($"Connection {sessionId} closed. Error: {error?.Message}");
                    }
                    return Task.CompletedTask;
                };

                await hubConnection.StartAsync();

                // send booking request

                var requestData = CreateRequestData(sessionId);
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(REQUEST_TIMEOUT_SECONDS));
                var response = await _httpClient.PostAsync($"Appointments", content, cts.Token);
                result.HttpRequestDt = DateTime.Now;

                if (response.IsSuccessStatusCode)
                {
                    result.RequestSuccess = true;
                    try
                    {
                        var (status, endTime) = await tcs.Task.WaitAsync(cts.Token);
                        result.Duration = endTime - startTime;
                        result.Status = status;
                        result.SignalRReceived = true;
                    } catch (OperationCanceledException)
                    {
                        result.Error = new TimeoutException($"SignalR response timed out for request {requestId}");
                        result.TimeoutType = "SignalR";
                        Console.WriteLine($"Request {requestId}: SignalR response timed out after successful HTTP request");
                    }
                } else
                {
                    result.RequestSuccess = false;
                    result.Error = new HttpRequestException($"HTTP request failed with status {response.StatusCode}");
                    result.TimeoutType = "HTTP";
                    Console.WriteLine($"Request {requestId}: HTTP request failed with status {response.StatusCode}");
                }
            }
            catch (OperationCanceledException)
            {
                result.TimeoutType = "Overall";
                result.Error = new TimeoutException($"Request {requestId} timed out");
                Console.WriteLine($"Request {requestId} timed out. RequestSuccess: {result.RequestSuccess}; HTTPRequest Timestamp: {result.HttpRequestDt};");
            }
            catch (Exception ex)
            {
                result.RequestSuccess = false;
                result.Error = ex;
                Console.WriteLine($"Request {requestId} failed: {ex.Message}");
            }
            finally
            {
                _results.Add(result);

                if (hubConnection != null)
                {
                    await hubConnection.StopAsync();
                    await hubConnection.DisposeAsync();
                }
            }
        }

        [Fact]
        public async Task RunTest()
        {
            int nTasks = 200;

            var tasks = new List<Task>();
            for (int i = 0; i < nTasks; i++)
            {
                string sessionId = $"{Guid.NewGuid()}";
                tasks.Add(SimulateBookingRequest(sessionId, i));
            }
            var startTime = DateTime.Now;
            await Task.WhenAll(tasks);
            var totalDuration = DateTime.Now - startTime;

            PrintResults(totalDuration);
        }
    }
}
