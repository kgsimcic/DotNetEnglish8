﻿using DotNetProject8.Models;
using DotNetProject8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest bookingRequest)
        {
            _logger.LogInformation("DN8: Redirecting to CreateBooking view...");

            List<AppointmentResponse> appointments = await _routingService.GetAppointments(bookingRequest.Consultant.Id, bookingRequest.Date);
            List<DateTime> takenAppointmentTimes = appointments.Select(a => a.AppointmentTime).ToList();

            BookingRequestModel bookingRequestModel = _producerService.CreateBookingModel(bookingRequest.Date, bookingRequest.Consultant, takenAppointmentTimes);

            return View(bookingRequestModel);
        }

/*        private long GenerateUniqueId()
        {
            int randomNumber = new Random().Next(1000, 9999);
            string dateString = $"{DateTime.Now:yyyyMMddHHmmss}";
            return Convert.ToInt64($"{dateString}{randomNumber}"); 
        }*/

        public async Task<IActionResult> EnqueueAppointment([FromForm] BookingRequestModel bookingRequestModel)
        {
            // bookingRequestModel.Appointment.AppointmentId = GenerateUniqueId();
            if (!ModelState.IsValid)
            {
                return View("CreateBooking", bookingRequestModel);
            }
            _logger.LogInformation("DN8: Appointment Creation Requested. Passing to Booking Service...");
            await _producerService.EnqueueBookingAsync(bookingRequestModel);

            // ViewBag.AppointmentId = bookingRequestModel.Appointment.AppointmentId;
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
