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

        public async Task<ActionResult> NewAppointment()
        {
            return View();
        }

        public async Task<ActionResult> ConfirmOrDenyAppointment()
        {
            return View();
        }
    }
}
