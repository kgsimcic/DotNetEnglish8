using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace ConsultantCalendarMicroservice.Services
{
    public class CalendarService : ICalendarService
    {
        public ConsultantCalendarDbContext DbContext { get; set; }

        public CalendarService(ConsultantCalendarDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<ConsultantCalendarModel> GetConsultantCalendar(int consultantId, int selectedMonth)
        {
            List<DateTime> consultantAvailableDates = await DbContext.ConsultantCalendars
                .Where(c => c.ConsultantId == consultantId && c.Date.Month == selectedMonth)
                .Select(o => o.Date).ToListAsync();

            ConsultantCalendarModel consultantCalendarModel = new ConsultantCalendarModel {
                MonthId = selectedMonth,
                ConsultantId = consultantId,
                AvailableDates = consultantAvailableDates
            };
            return consultantCalendarModel;
        }

    }
}
