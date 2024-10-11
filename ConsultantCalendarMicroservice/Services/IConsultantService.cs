using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Models;

namespace ConsultantCalendarMicroservice.Services
{
    public interface IConsultantService
    {
        public Task<List<ConsultantModel>> GetConsultants();
    }
}
