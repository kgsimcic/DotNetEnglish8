using ConsultantCalendarMicroservice.Models;
using ConsultantCalendarMicroservice.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantCalendarMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultantCalendarController : ControllerBase
    {

        private readonly ILogger<ConsultantCalendarController> _logger;
        private readonly IConsultantService _consultantService;
        private readonly ICalendarService _calendarService;

        public ConsultantCalendarController(ILogger<ConsultantCalendarController> logger, 
            IConsultantService consultantService, 
            ICalendarService calendarService)
        {
            _logger = logger;
            _calendarService = calendarService;
            _consultantService = consultantService;
        }

        [HttpGet("consultants")]
        public async Task<ActionResult> GetAllConsultants()
        {
            _logger.LogInformation("Connected to endpoint /consultants! Retrieving all info...");
            IEnumerable<ConsultantModel> result = await _consultantService.GetConsultants();
            return Ok(result);
        }

        [HttpGet("consultants/{selectedMonth}")]
        public async Task<ActionResult> GetConsultantCalendars(int selectedMonth = 3)
        {
            _logger.LogInformation($"Connected to endpoint /consultants/{selectedMonth}!");

            try
            {
                List<ConsultantCalendarModel> result = await _calendarService.GetConsultantCalendars(selectedMonth);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                // not sure if this will be needed.
                return NotFound($"A consultant with ID was not found.");
            }
        }
    }
}
