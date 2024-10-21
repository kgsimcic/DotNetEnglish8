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

            await _dbContext.Patients.AddAsync(patient);
            await _dbContext.SaveChangesAsync();

            Appointment appointment = new Appointment
            {
                StartDateTime = appointmentTime,
                EndDateTime = appointmentTime.AddHours(1),
                ConsultantId = bookingModel.Appointment.ConsultantId,
                PatientId = patient.Id
            };

            var possibleAppointments = await _dbContext.Appointments.Where(a => a.StartDateTime > appointment.EndDateTime 
            || a.EndDateTime > appointment.StartDateTime).ToListAsync();

            if (possibleAppointments.Any())
            {
                throw new DBConcurrencyException("Timeslot conflict");
            }

            await _dbContext.Appointments.AddAsync(appointment);
            int result = await _dbContext.SaveChangesAsync();

            // now that time-sensitive appointment is booked, other entities can be crafted and added (should I 
            // split tis into another method?)

          
            return (result);
        }

        public async Task<IEnumerable<AppointmentDetails>> GetBookings(int consultantId, int month)
        {
            int year = DateTime.Now.Year;
            if (month < DateTime.Now.Month)
            {
                year = year + 1;
            }
            DateTime targetMonth = new (year, month, 1);

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
