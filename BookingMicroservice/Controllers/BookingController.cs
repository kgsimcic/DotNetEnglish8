using BookingMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using BookingMicroservice.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using SchedulingWorkerService;

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
        public async Task<ActionResult> CreateAppointment([FromBody]BookingModel bookingModel)
        {
            _logger.LogInformation("BookingMS: Attempting to create appointment....");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int result = await _bookingService.CreateBooking(bookingModel);

            if (result == 0)
            {
                return Conflict("This appointment is no longer available. Please refresh the page and try again.");
            } else
            {
                return Created();
            }
        }

        [HttpGet("bookings/{consultantId}-{month}")]
        public async Task<ActionResult> GetAppointments(int consultantId, int month)
        {
            _logger.LogInformation($"BookingMS: Fetching Bookings for next {month} for consultant ID#{consultantId}....");
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            return Ok(await _bookingService.GetBookings(consultantId, month));
        }
    }
}
