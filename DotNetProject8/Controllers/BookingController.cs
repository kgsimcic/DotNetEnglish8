using DotNetProject8.Models;
using DotNetProject8.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetProject8.Controllers
{
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IRoutingService _routingService;

        public BookingController(ILogger<BookingController> logger, IRoutingService routingService)
        {
            _logger = logger;
            _routingService = routingService;
        }

        public async Task<ActionResult> NewAppointment(int selectedConsultantId, string selectedConsultantName, DateTime selectedDate)
        {
            _logger.LogInformation("DN8: Redirecting to CreateBooking view...");
            return View("CreateBooking");
        }

        public async Task<ActionResult> ConfirmOrDenyAppointment([FromBody] BookingModel bookingModel)
        {
            _logger.LogInformation("DN8: Appointment Creation Requested. Passing to Booking Service...");
            // _routingService.CreateAppointment?
            return View();
        }
    }
}
