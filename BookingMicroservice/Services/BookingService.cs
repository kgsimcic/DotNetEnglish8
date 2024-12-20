using BookingMicroservice.Entities;
using BookingMicroservice.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Net.Http;
using System.Xml;

namespace BookingMicroservice.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _dbContext;
        private readonly ILogger<BookingService> _logger;
        private readonly SseService _sseService;
        private readonly IMemoryCache _memoryCache;

        public BookingService(ILogger<BookingService> logger, BookingDbContext dbContext, SseService sseService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _dbContext = dbContext;
            _sseService = sseService;
            _memoryCache = memoryCache;
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

        public async Task<int> GetAppointmentCountAsync(string cacheKey, DateTime startDateTime)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out int count))
            {
                count = await _dbContext.Appointments.CountAsync(
                a => a.StartDateTime.Hour == startDateTime.Hour &&
                a.StartDateTime.Date == startDateTime.Date);
                /*var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)); 
                _memoryCache.Set(cacheKey, count, cacheEntryOptions); */
            }
            return count; 
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

            string key = $"AppointmentCount_{appointment.StartDateTime:yyyyMMddHHmm}";
            int count = await GetAppointmentCountAsync(key, appointment.StartDateTime);

            if (count > 0)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _memoryCache.Set(key, count, cacheEntryOptions);

                _logger.LogInformation($"Appointment #{appointment.AppointmentUniqueId} blocked due to double Booking.");
                appointmentStatusResponse.Status = "Failed";
            }
            else
            {
                _memoryCache.Remove(key);
                await _dbContext.Appointments.AddAsync(appointment);
                int result = await _dbContext.SaveChangesAsync();
                if (result == 0)
                {
                    _logger.LogInformation("SaveChangesAsync Failed in BookingDbContext. Why???");
                    appointmentStatusResponse.Status = "Failed";
                }
                else
                {
                    _logger.LogInformation($"Appointment #{appointment.AppointmentUniqueId} booked.");
                    appointmentStatusResponse.Status = "Completed";
                }
            }

            await _sseService.SendUpdateAsync(appointment.AppointmentUniqueId, appointmentStatusResponse);
        }
    }
}
