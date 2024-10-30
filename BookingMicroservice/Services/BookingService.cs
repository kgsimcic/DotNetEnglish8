using BookingMicroservice.Entities;
using BookingMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml;

namespace BookingMicroservice.Services
{
    public class BookingService : IBookingService
    {
        private BookingDbContext _dbContext;
        private readonly ILogger<BookingService> _logger;

        public BookingService(ILogger<BookingService> logger, BookingDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<int> CreateBooking(BookingModel bookingModel)
        {
            DateTime appointmentTime = bookingModel.Appointment.AppointmentTime;
            var test = bookingModel.Patient.PatientId;

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

            var possibleAppointments = await _dbContext.Appointments.Where(a => a.StartDateTime > appointment.EndDateTime 
            || a.EndDateTime > appointment.StartDateTime).ToListAsync();

            if (possibleAppointments.Any())
            {
                return 0;
            }
            else
            {
                await _dbContext.Appointments.AddAsync(appointment);
                int result = await _dbContext.SaveChangesAsync();
                if (result == 0)
                {
                    _logger.LogInformation("SaveChangesAsync Failed in BookingDbContext. Why???");
                }
                return (result);
            }
        }


        public async Task<IEnumerable<AppointmentDetails>> GetBookings(int consultantId, int selectedMonth)
        {
            int year = DateTime.Now.Year;
            if (selectedMonth < DateTime.Now.Month)
            {
                year = year + 1;
            }
            DateTime targetMonth = new(year, selectedMonth, 1);

            var appointmentsInMonth = await _dbContext.Appointments.Where(a => a.ConsultantId == consultantId
            && a.StartDateTime.Month == targetMonth.Month && a.StartDateTime.Year == targetMonth.Year).ToListAsync();

            return appointmentsInMonth.Select(a => new AppointmentDetails
            {
                AppointmentDate = a.StartDateTime,
                AppointmentTime = a.StartDateTime
            });
        }
    }
}
