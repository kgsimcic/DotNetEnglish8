using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Models;

namespace ConsultantCalendarMicroservice.Services
{
    public interface ICalendarService
    {
        public Task<List<ConsultantCalendarModel>> GetConsultantCalendars(int selectedMonth);
    }
}
