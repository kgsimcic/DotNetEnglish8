using DotNetProject8.ViewModels;
using DotNetProject8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;
using System.Diagnostics;
using System.Drawing.Printing;

namespace DotNetProject8.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoutingService _routingService;

        public HomeController(ILogger<HomeController> logger, IRoutingService routingService)
        {
            _logger = logger;
            _routingService = routingService;
        }

        public async Task<IActionResult> Index()
        {

            List<ConsultantViewModel>? consultantModels = await _routingService.GetConsultantsAsync();
            if (consultantModels == null)
            {
                _logger.LogWarning("nothing found");
            }

            return View(new ConsultantViewModelList
            {
                Consultants = consultantModels
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
