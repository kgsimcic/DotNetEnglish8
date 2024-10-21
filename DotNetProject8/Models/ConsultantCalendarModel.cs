using System.ComponentModel.DataAnnotations;

namespace DotNetProject8.Models
{
    public class ConsultantCalendarModel
    {
        public int MonthId { get; set; }
        public int ConsultantId { get; set; }

        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();

    }
}
