using BookingMicroservice.Entities;
using BookingMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Http;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingMicroservice.Services
{
    public class BookingService : IBookingService
    {
        private BookingDbContext _dbContext;
        private readonly ILogger<BookingService> _logger;
        private static readonly HttpClient httpClient = new HttpClient();

        public BookingService(ILogger<BookingService> logger, BookingDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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

        // FIX! 
        public async Task ProcessBookingAsync(BookingModel bookingModel)
        {

            DateTime appointmentTime = bookingModel.Appointment.AppointmentTime;

            Patient patient = new Patient
            {
                Fname = bookingModel.Patient.PatientFName,
                Lname = bookingModel.Patient.PatientLName,
                Address1 = bookingModel.Patient.AddressLine1,
                Address2 = bookingModel.Patient.AddressLine2,
                City = bookingModel.Patient.City,
                Postcode = bookingModel.Patient.Postcode
            };

            Appointment appointment = new Appointment
            {
                StartDateTime = appointmentTime,
                EndDateTime = appointmentTime.AddHours(1),
                ConsultantId = bookingModel.Appointment.ConsultantId,
                PatientId = patient.Id,
                Patient = patient
            };

            // FIX: session Id.
            int sessionId = 1;

            // response stuff 

            AppointmentStatusResponse appointmentStatusResponse = new()
            {
                AppointmentId = sessionId,
                Status = null
            };

            // Double Booking Logic

            var possibleAppointments = await _dbContext.Appointments.Where(
                a => a.StartDateTime.Hour == appointment.StartDateTime.Hour &&
                a.StartDateTime.Date == appointment.StartDateTime.Date).ToListAsync();


            if (possibleAppointments.Any())
            {
                _logger.LogInformation($"Error: Appointment #{sessionId} failed - Double Booking.");
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
                    _logger.LogInformation($"Appointment #{sessionId} completed.");
                    appointmentStatusResponse.Status = "Completed";
                }
            }

            // FIX: need to send to associated sessionId only.
            // signalR response
/*            var response = await httpClient.PostAsJsonAsync("https://localhost:5001/appointmentHub", appointmentStatusResponse);
            response.EnsureSuccessStatusCode();*/
        }
    }
}
