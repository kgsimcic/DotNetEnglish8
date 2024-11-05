using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using static Azure.Core.HttpHeader;

namespace ConsultantCalendarMicroservice.Services
{
    public class CalendarService : ICalendarService
    {
        private ConsultantCalendarDbContext _dbContext { get; set; }

        public CalendarService(ConsultantCalendarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ConsultantCalendarModel>> GetConsultantCalendars(int selectedMonth)
        {
            var query = _dbContext.ConsultantCalendars
                .Where(c => c.Date.Month == selectedMonth).GroupBy(c => c.ConsultantId);

            var groupedConsultantCalendars = await query.ToListAsync();

            List<ConsultantCalendarModel> consultantCalendarModels = new ();

            foreach (var group in groupedConsultantCalendars)
            {

                int ConsultantId = group.Key;
                List<DateTime> consultantAvailableDates = group.Select(c => c.Date).ToList();

                consultantCalendarModels.Add(new ConsultantCalendarModel()
                {
                    MonthId = selectedMonth,
                    ConsultantId = ConsultantId,
                    AvailableDates = consultantAvailableDates
                });
            };
            return consultantCalendarModels;
        }

    }
}
