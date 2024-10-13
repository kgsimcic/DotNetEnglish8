using ConsultantCalendarMicroservice.Models;
using ConsultantCalendarMicroservice.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantCalendarMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet(Name = "/api/consultants")]
        public async Task<ActionResult> GetAllConsultants()
        {
            _logger.LogInformation("Connected to endpoint /consultants! Retrieving all info...");
            IEnumerable<ConsultantModel> result = await _consultantService.GetConsultants();
            return Ok(result);
        }

        [HttpGet(Name = "api/consultants/{id}/calendar")]
        public async Task<ActionResult> GetConsultantCalendar(int consultantId, int selectedMonth)
        {
            _logger.LogInformation($"Connected to endpoint /consultants/{consultantId}/calendar! Retrieving requested consultant calendar.");

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
