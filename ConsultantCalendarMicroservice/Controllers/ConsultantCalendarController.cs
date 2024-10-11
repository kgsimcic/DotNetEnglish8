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

        [HttpGet(Name = "/consultants")]
        public async Task<ActionResult> GetAllConsultants()
        {
            _logger.LogInformation("Connected to endpoint /consultants! Retrieving all info...");
            List<ConsultantModel> result = await _consultantService.GetConsultants();
            return Ok(result);
        }

        [HttpGet(Name = $"consultants/{Id}/calendar")]
        public async Task<ActionResult> GetConsultantCalendar(int Id)
        {
            _logger.LogInformation($"Connected to endpoint /consultants/{Id}/calendar! Retrieving requested consultant calendar.");
            if (Id == 0) { return  BadRequest("No consultant ID specified."); }

            try
            {
                string consultantName = _consultantService.GetNameById(Id);
                ConsultantCalendarModel result = await _calendarService.GetConsultantCalendar(consultantName);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"A consultant with ID = {Id} was not found.");
            }
        }
    }
}
