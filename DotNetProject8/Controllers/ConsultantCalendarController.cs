using DotNetProject8.Services;
using DotNetProject8.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DotNetProject8.Controllers
{
    public class ConsultantCalendarController : Controller
    {
        private readonly ILogger<ConsultantCalendarController> _logger;
        private readonly IRoutingService _routingService;

        public ConsultantCalendarController(ILogger<ConsultantCalendarController> logger, IRoutingService routingService)
        {
            _logger = logger;
            _routingService = routingService;
        }

        public async Task<ActionResult> GetConsultantCalendars(int selectedMonth = 3)
        {
            int year = DateTime.Now.Year;
            if (selectedMonth < DateTime.Now.Month)
            {
                year = year + 1;
            }
            DateTime targetMonthYear = new(year, selectedMonth, 1);

            List<ConsultantViewModel>? consultants = await _routingService.GetConsultantsAsync();
            List<ConsultantCalendarViewModel>? consultantCalendars = await _routingService.GetConsultantCalendars(selectedMonth);
            ConsultantViewModelList consultantViewModelList = new()
            {
                ConsultantCalendars = consultantCalendars,
                Consultants = consultants,
                SelectedConsultantId = 1,
                ConsultantsList = new SelectList(consultants, "Id", "Fname")
            };

            ViewBag.minDate = targetMonthYear;
            ViewBag.maxDate = new DateTime(targetMonthYear.Year, targetMonthYear.Month,
                DateTime.DaysInMonth(targetMonthYear.Year, targetMonthYear.Month));
            return View(consultantViewModelList);
        }

    }
}
