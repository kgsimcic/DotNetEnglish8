using DotNetProject8.Models;
using DotNetProject8.ViewModels;

namespace DotNetProject8.Services
{
    public interface IRoutingService
    {
        public Task<List<ConsultantViewModel>?> GetConsultantsAsync();
        public Task<List<ConsultantCalendarViewModel>?> GetConsultantCalendars(int selectedMonth);
        public Task<List<AppointmentModel>?> GetAppointments(int consultantId, DateTime selectedDate);

    }
}
