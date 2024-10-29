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
        // private readonly BookingWorkerService

        public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
            
        }

        [HttpPost("bookings")]
        public async Task<ActionResult> CreateAppointment([FromBody]BookingModel bookingModel)
        {
            _logger.LogInformation("Attempting to create appointment....");
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

        [HttpGet("bookings")]
        public async Task<ActionResult> GetAppointments(int ConsultantId, int month)
        {
            _logger.LogInformation($"Fetching Bookings for next {month} for consultant ID#{ConsultantId}....");
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            // real result: await _bookingService.GetBookings(ConsultantId, month)
            return Ok();
        }
    }
}
