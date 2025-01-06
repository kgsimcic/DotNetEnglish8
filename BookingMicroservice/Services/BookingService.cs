using BookingMicroservice.Entities;
using BookingMicroservice.Models;
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
        private readonly IMemoryCache _memoryCache;

        public BookingService(ILogger<BookingService> logger, BookingDbContext dbContext, IMemoryCache memoryCache)
        {
            _logger = logger;
            _dbContext = dbContext;
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

        public async Task<string> ProcessBookingAsync(BookingModel bookingModel)
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
                Patient = patient
            };

            string connectionId = bookingModel.Appointment.ConnectionId;
            string status;

            string key = $"AppointmentCount_{appointment.StartDateTime:yyyyMMddHHmm}";
            int count = await GetAppointmentCountAsync(key, appointment.StartDateTime);

            if (count > 0)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _memoryCache.Set(key, count, cacheEntryOptions);

                _logger.LogInformation($"Appointment #{connectionId} blocked due to double Booking.");
                status = "Failed";
            }
            else
            {
                _memoryCache.Remove(key);
                await _dbContext.Appointments.AddAsync(appointment);
                int result = await _dbContext.SaveChangesAsync();
                if (result == 0)
                {
                    _logger.LogInformation("SaveChangesAsync Failed in BookingDbContext. Why???");
                    status = "Failed";
                }
                else
                {
                    _logger.LogInformation($"Appointment #{connectionId} booked.");
                    status = "Completed";
                }
            }

            // await _sseService.SendUpdateAsync(appointment.AppointmentUniqueId, appointmentStatusResponse);
            // await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveStatusUpdate", status);
            return status;
        }
    }
}
