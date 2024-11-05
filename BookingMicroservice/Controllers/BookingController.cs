using BookingMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using BookingMicroservice.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure.Core;

namespace BookingMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {

        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;
        private readonly BookingWorkerService _workerService;

        public BookingController(ILogger<BookingController> logger, IBookingService bookingService, BookingWorkerService bookingWorkerService)
        {
            _logger = logger;
            _bookingService = bookingService;
            _workerService = bookingWorkerService;
        }

        [HttpPost("bookings")]
        public async Task<ActionResult> QueueBooking([FromBody]BookingModel bookingModel)
        {
            _logger.LogInformation("BookingMS: Attempting to create appointment....");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await Task.Run(() => _workerService.EnqueueBooking(bookingModel));
            return Ok(bookingModel.Appointment.AppointmentId);
        }

        [HttpGet("bookings/status/{id}")]
        public async Task<ActionResult> GetBookingStatus(Guid id, int timeoutMs = 300)
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
        }

        [HttpGet("bookings/{month}")]
        public async Task<ActionResult> GetAppointments(int month)
        {
            _logger.LogInformation($"BookingMS: Fetching Bookings for next {month}....");
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            return Ok(await _bookingService.GetBookings(month));
        }
    }
}
