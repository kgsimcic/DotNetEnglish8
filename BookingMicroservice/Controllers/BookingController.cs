using BookingMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using BookingMicroservice.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookingMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {

        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;

        public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        [HttpPost(Name = "/api/bookings")]
        public async Task<ActionResult> CreateAppointment([FromBody]BookingModel bookingModel)
        {
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
                return Conflict("This appointment is no longer available. Please refresh the page and try again.");
            }
        }
    }
}
