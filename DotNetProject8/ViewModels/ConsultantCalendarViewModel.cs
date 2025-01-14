using System.ComponentModel.DataAnnotations;

namespace DotNetProject8.ViewModels
{
    public class ConsultantCalendarViewModel
    {
        public int ConsultantId { get; set; }
        public int MonthId { get; set; }

        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();

    }
}
