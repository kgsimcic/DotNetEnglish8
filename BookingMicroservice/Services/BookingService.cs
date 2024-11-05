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

        public async Task<IEnumerable<AppointmentDetails>> GetBookings(int selectedMonth)
        {
            int year = DateTime.Now.Year;
            if (selectedMonth < DateTime.Now.Month)
            {
                year = year + 1;
            }
            DateTime targetMonth = new(year, selectedMonth, 1);

            var appointmentsInMonth = await _dbContext.Appointments.Where(a => a.StartDateTime.Month == targetMonth.Month && a.StartDateTime.Year == targetMonth.Year).ToListAsync();

            return appointmentsInMonth.Select(a => new AppointmentDetails
            {
                AppointmentDate = a.StartDateTime,
                AppointmentTime = a.StartDateTime
            });
        }


    }
}
