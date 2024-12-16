using BookingMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using BookingMicroservice.Services;

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
