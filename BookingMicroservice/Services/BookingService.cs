using BookingMicroservice.Entities;
using BookingMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace BookingMicroservice.Services
{
    public class BookingService
    {
        public BookingDbContext DbContext { get; set; }
        private readonly ILogger<BookingService> _logger;

        public BookingService(ILogger<BookingService> logger, BookingDbContext dbContext)
        {
            _logger = logger;
            DbContext = dbContext;
        }

        public async Task<bool> CreateBooking(BookingModel bookingModel)
        {
            DateTime appointmentTime = bookingModel.Appointment.AppointmentTime;

            // craft entities from booking model - patient, appointment, consultantcalendar
            Appointment appointment = new Appointment
            {
                StartDateTime = appointmentTime,
                EndDateTime = appointmentTime.AddHours(1),
                ConsultantId = bookingModel.Consultant.ConsultantId,
                PatientId = bookingModel.Patient.PatientId,
            };

            try
            {

                // need to fix all this -- needs to check if appointment with timeslot has already
                // been created.

                DbContext.Appointments.Add(appointment);
                await DbContext.SaveChangesAsync();
                return (true);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    var clientValues = entry.Entity;
                    var databaseValues = entry.GetDatabaseValues();

                    if (databaseValues == null)
                    {
                        // Conflicting appointment was already created
                        _logger.LogError("The entity has been deleted.");
                    }
                }
                return (false);
            }
        }
    }
}
