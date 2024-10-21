using BookingMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using BookingMicroservice.Services;
using Microsoft.AspNetCore.Http.HttpResults;

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

        [HttpPost("/bookings")]
        public async Task<ActionResult> CreateAppointment([FromBody]BookingModel bookingModel)
        {
            _logger.LogInformation("Attempting to create appointment....");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _bookingService.CreateBooking(bookingModel);
                // fill Created() with a result object passing back appointment details
                return Created();
            }
            catch (Exception) {
                _logger.LogWarning("Concurrency Exception occurred. Cancelling appointment creation.");
                return Conflict("This appointment is no longer available. Please refresh the page and try again.");
            }
        }

        [HttpGet("/bookings")]
        public async Task<ActionResult> GetAppointments(int ConsultantId, int month)
        {
            _logger.LogInformation($"Fetching Bookings for next {month} for consultant ID#{ConsultantId}....");
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            return Ok(await _bookingService.GetBookings(ConsultantId, month));
        }
    }
}
