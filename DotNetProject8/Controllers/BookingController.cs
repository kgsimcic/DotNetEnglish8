using DotNetProject8.Models;
using DotNetProject8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace DotNetProject8.Controllers
{
    [Controller]
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IRoutingService _routingService;
        private readonly BookingProducerService _producerService;

        public BookingController(ILogger<BookingController> logger, IRoutingService routingService, BookingProducerService producerService)
        {
            _logger = logger;
            _routingService = routingService;
            _producerService = producerService;
        }


        public IActionResult RedirectUrl([FromBody] BookingRequest bookingRequest)
        {
            _logger.LogInformation("DN8: Getting url...");

            TempData["Date"] = JsonConvert.SerializeObject(bookingRequest.Date);
            TempData["Consultant"] = JsonConvert.SerializeObject(bookingRequest.Consultant);

            var url = Url.Action("CreateBooking", "Booking");
            return Json(new { url });
        }

        public async Task<IActionResult> CreateBooking()
        {
            _logger.LogInformation("DN8: Redirecting to CreateBooking view...");

            if (TempData["Date"] != null && TempData["Consultant"] != null)
            {

                DateTime date = JsonConvert.DeserializeObject<DateTime>(TempData["Date"].ToString());
                ConsultantModel consultant = JsonConvert.DeserializeObject<ConsultantModel>(TempData["Consultant"].ToString());

                List<AppointmentResponse> appointments = await _routingService.GetAppointments(consultant.Id, date);
                List<DateTime> takenAppointmentTimes = appointments.Select(a => a.AppointmentTime).ToList();

                List<DateTime> appointmentTimes = new();

                DateTime startTime = date.AddHours(8);

                for (int i = 0; i < 10; i++)
                {
                    if (!takenAppointmentTimes.Contains(startTime.AddHours(i)))
                    {
                        DateTime test = startTime.AddHours(i);
                        appointmentTimes.Add(test);
                    }
                }

                AppointmentRequestDetails appointmentRequestDetails = new()
                {
                    AppointmentDate = date,
                    AppointmentTimes = new SelectList(appointmentTimes),
                    SelectedAppointmentTime = appointmentTimes.Min()
                };

                ConsultantRequestDetails consultantRequestDetails = new()
                {
                    ConsultantId = consultant.Id,
                    ConsultantName = consultant.Fname + ' ' + consultant.Lname,
                    ConsultantSpeciality = consultant.Speciality
                };

                BookingRequestModel bookingRequestModel = new()
                {
                    Consultant = consultantRequestDetails,
                    Appointment = appointmentRequestDetails,
                    Patient = new()
                };

                return View(bookingRequestModel);
            }
            return View();
        }

        private long GenerateUniqueId()
        {
            int randomNumber = new Random().Next(1000, 9999);
            string dateString = $"{DateTime.Now:yyyyMMddHHmmss}";
            return Convert.ToInt64($"{dateString}{randomNumber}"); 
        }

        public async Task<IActionResult> EnqueueAppointment([FromForm] BookingRequestModel bookingRequestModel)
        {
            bookingRequestModel.Appointment.AppointmentId = GenerateUniqueId();
            if (!ModelState.IsValid)
            {
                return View("CreateBooking", bookingRequestModel);
            }
            _logger.LogInformation("DN8: Appointment Creation Requested. Passing to Booking Service...");
            await _producerService.EnqueueBookingAsync(bookingRequestModel);

            ViewBag.AppointmentId = bookingRequestModel.Appointment.AppointmentId;
            return View("AppointmentPending");
        }

        // For testing only: call via api endpoint. 
        [HttpPost("Appointments")]
        public async Task<IActionResult> ApiEnqueueAppointment([FromBody] BookingRequestModel bookingRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Modelstate invalid!");
            }
            _logger.LogInformation("DN8: Appointment Creation Requested via API. Passing to Booking Service...");
            await _producerService.EnqueueBookingAsync(bookingRequestModel);

            return Ok();
        }

        public IActionResult Completed()
        {
            return View("AppointmentConfirmation");
        }

        public IActionResult Failed()
        {
            return View("AppointmentError");
        }
    }
}
