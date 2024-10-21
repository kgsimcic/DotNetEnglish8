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
            _logger.LogInformation("Booking Request Form loading...");
            return View();
        }

        public async Task<ActionResult> ConfirmOrDenyAppointment()
        {
            _logger.LogInformation("Appointment Creation Requested. Passing to Booking Service...");
            return View();
        }
    }
}
