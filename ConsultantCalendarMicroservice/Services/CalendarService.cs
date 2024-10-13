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
            // using consultant name, get calendar for them. 
            List<ConsultantCalendar> consultantCalendars = await DbContext.FindAsync<ConsultantCalendar>(consultantId);
            // make sure to create model objects off of them before returning.
            List<>


            return Consultants.Select(c => new ConsultantCalendarModel
            {
                Id,
                ConsultantName = consultantName,
                AvailableDates = 
            });
        }

        
    }
}
