using Azure.Core;
using BookingMicroservice.Entities;
using BookingMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace BookingMicroservice.Services
{
   
    public class BookingWorkerService 
    {
        private readonly ILogger<BookingWorkerService> _logger;
        private readonly ConcurrentQueue<BookingModel> _bookingQueue = new();
        private readonly ConcurrentDictionary<Guid, string> _bookingStatuses = new();
        private IBookingService _bookingService;
        private BookingDbContext _dbContext;

        public BookingWorkerService(ILogger<BookingWorkerService> logger, IBookingService bookingService, 
            BookingDbContext bookingDbContext, ConcurrentDictionary<Guid, string> bookingStatuses)
        {
            _logger = logger;
            _bookingService = bookingService;
            _dbContext = bookingDbContext;
            _bookingStatuses = bookingStatuses;
        }

        public void EnqueueBooking(BookingModel bookingModel)
        {
            _bookingQueue.Enqueue(bookingModel);
            _bookingStatuses[bookingModel.Appointment.AppointmentId] = "Queued";
        }

        public string GetBookingStatus(Guid id)
        {
            _bookingStatuses.TryGetValue(id, out var status);
            return status ?? "Not Found";
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                while (_bookingQueue.TryDequeue(out var bookingModel))
                {
                    await ProcessBookingAsync(bookingModel);
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(100, stoppingToken);
            }
            _logger.LogInformation("BookingWorkerService stopped.");
        }

        private async Task ProcessBookingAsync(BookingModel bookingModel)
        {
            _logger.LogInformation($"Booking appointment ID#{bookingModel.Appointment.AppointmentId}");
            _bookingStatuses[bookingModel.Appointment.AppointmentId] = "Processing";

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
                _logger.LogInformation($"Error: Appointment #{bookingModel.Appointment.AppointmentId} failed - Double Booking.");
                _bookingStatuses[bookingModel.Appointment.AppointmentId] = "Failed";
            } else {
                await _dbContext.Appointments.AddAsync(appointment);
                int result = await _dbContext.SaveChangesAsync();
                if (result == 0)
                {
                    _logger.LogInformation("SaveChangesAsync Failed in BookingDbContext. Why???");
                    _bookingStatuses[bookingModel.Appointment.AppointmentId] = "Failed";
                } else
                {
                    _logger.LogInformation($"Appointment #{bookingModel.Appointment.AppointmentId} completed.");
                    _bookingStatuses[bookingModel.Appointment.AppointmentId] = "Completed";
                }
            }
        }
    }

}
