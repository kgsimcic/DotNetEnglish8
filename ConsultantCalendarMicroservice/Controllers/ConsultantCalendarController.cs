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

        [HttpGet("/consultants")]
        public async Task<ActionResult> GetAllConsultants()
        {
            _logger.LogInformation("Connected to endpoint /consultants! Retrieving all info...");
            IEnumerable<ConsultantModel> result = await _consultantService.GetConsultants();
            return Ok(result);
        }

        [HttpGet("/consultants/{consultantId}")]
        public async Task<ActionResult> GetConsultantCalendar(int consultantId, int selectedMonth = 3)
        {
            _logger.LogInformation($"Connected to endpoint /consultants/{consultantId}! Retrieving requested consultant calendar.");

            try
            {
                ConsultantCalendarModel result = await _calendarService.GetConsultantCalendar(consultantId, selectedMonth);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"A consultant with ID = {consultantId} was not found.");
            }
        }
    }
}
