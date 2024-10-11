using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Models;

namespace ConsultantCalendarMicroservice.Services
{
    public interface ICalendarService
    {
        public Task<ConsultantCalendarModel> GetConsultantCalendar(string consultantName);
    }
}
