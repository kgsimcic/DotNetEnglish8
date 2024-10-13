using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Models;

namespace ConsultantCalendarMicroservice.Services
{
    public interface IConsultantService
    {
        public Task<IEnumerable<ConsultantModel>> GetConsultants();
    }
}
