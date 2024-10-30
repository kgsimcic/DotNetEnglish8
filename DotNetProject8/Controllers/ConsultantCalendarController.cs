﻿using DotNetProject8.Models;
using DotNetProject8.Services;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<ActionResult> GetConsultants()
        {
            List<ConsultantModel>? consultants = await _routingService.GetConsultantsAsync();
            // ViewData["Products"] = consultants;


            return View();
        }

        public async Task<ActionResult> GetConsultantCalendar(int consultantId, int selectedMonth = 3)
        {
            return View(await _routingService.GetConsultantCalendar(consultantId, selectedMonth));
        }

    }
}
