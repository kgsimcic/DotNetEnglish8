using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultantCalendarMicroservice.Services
{
    public class ConsultantService
    {

        private ConsultantCalendarDbContext DbContext { get; set; }

        public ConsultantService(ConsultantCalendarDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<IEnumerable<ConsultantModel>> GetConsultants()
        {
            List<Consultant> Consultants = await DbContext.Set<Consultant>().ToListAsync();

            return Consultants.Select(c => new ConsultantModel
            {
                Id = c.Id,
                Fname = c.Fname,
                Lname = c.Lname,
                Speciality = c.Speciality
            });
        }

    }
}
