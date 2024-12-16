using BookingMicroservice.Entities;
using BookingMicroservice.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Http;
using System.Xml;

namespace BookingMicroservice.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _dbContext;
        private readonly ILogger<BookingService> _logger;
        private static readonly HttpClient httpClient = new ();
        private readonly SseService _sseService;

        public BookingService(ILogger<BookingService> logger, BookingDbContext dbContext, SseService sseService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _sseService = sseService;
        }

        public async Task<IEnumerable<AppointmentDetails>> GetBookings(DateTime selectedDate)
        {

            var appointmentsInMonth = await _dbContext.Appointments.Where(a => a.StartDateTime.Date == selectedDate).ToListAsync();

            return appointmentsInMonth.Select(a => new AppointmentDetails
            {
                AppointmentDate = a.StartDateTime.Date,
                AppointmentTime = a.StartDateTime,
                ConsultantId = a.ConsultantId
            });
        }

        public async Task ProcessBookingAsync(BookingModel bookingModel)
        {
            DateTime appointmentTime = bookingModel.Appointment.AppointmentTime;

            Patient patient = new ()
            {
                Fname = bookingModel.Patient.PatientFName,
                Lname = bookingModel.Patient.PatientLName,
                Address1 = bookingModel.Patient.AddressLine1,
                Address2 = bookingModel.Patient.AddressLine2,
                City = bookingModel.Patient.City,
                Postcode = bookingModel.Patient.Postcode
            };

            Appointment appointment = new ()
            {
                StartDateTime = appointmentTime,
                EndDateTime = appointmentTime.AddHours(1),
                ConsultantId = bookingModel.Appointment.ConsultantId,
                PatientId = patient.Id,
                AppointmentUniqueId = bookingModel.Appointment.AppointmentId,
                Patient = patient
            };

            AppointmentStatusResponse appointmentStatusResponse = new()
            {
                AppointmentId = appointment.AppointmentUniqueId,
                Status = null
            };

            var possibleAppointments = await _dbContext.Appointments.Where(
                a => a.StartDateTime.Hour == appointment.StartDateTime.Hour &&
                a.StartDateTime.Date == appointment.StartDateTime.Date).ToListAsync();


            if (possibleAppointments.Any())
            {
                _logger.LogInformation($"Error: Appointment #{appointment.AppointmentUniqueId} failed - Double Booking.");
                appointmentStatusResponse.Status = "Failed";
            }
            else
            {
                await _dbContext.Appointments.AddAsync(appointment);
                int result = await _dbContext.SaveChangesAsync();
                if (result == 0)
                {
                    _logger.LogInformation("SaveChangesAsync Failed in BookingDbContext. Why???");
                    appointmentStatusResponse.Status = "Failed";
                }
                else
                {
                    _logger.LogInformation($"Appointment #{appointment.AppointmentUniqueId} completed.");
                    appointmentStatusResponse.Status = "Completed";
                }
            }

            await _sseService.SendUpdateAsync(appointment.AppointmentUniqueId, appointmentStatusResponse);
        }
    }
}
