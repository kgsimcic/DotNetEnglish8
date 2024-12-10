using BookingMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using BookingMicroservice.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;

namespace BookingMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {

        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;

        public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        /*[HttpGet("bookings/status/{id}")]
        public async Task<ActionResult> GetBookingStatus(int id, int timeoutMs = 300)
        {
            const int checkInterval = 100;
            int elapsed = 0;

            while (elapsed < timeoutMs)
            {
                var status = _workerService.GetBookingStatus(id);
                if (status != "Queued" && status != "Processing")
                {
                    return Ok(new { BookingId = id, Status = status });
                }

                await Task.Delay(checkInterval);
                elapsed += checkInterval;
            }

            return Ok(new { BookingId = id, Status = "Pending" });
        }*/

        [HttpGet("bookings/{date}")]
        public async Task<ActionResult> GetAppointments(string date)
        {
            _logger.LogInformation($"BookingMS: Fetching Bookings for {date}....");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DateTime selectedDate = DateTimeOffset.Parse(date).Date;
            return Ok(await _bookingService.GetBookings(selectedDate));
        }
    }
}
