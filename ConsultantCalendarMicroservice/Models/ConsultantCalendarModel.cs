using System.ComponentModel.DataAnnotations;

namespace ConsultantCalendarMicroservice.Models
{
    public class ConsultantCalendarModel
    {
        [Key]
        public int Id { get; set; }
        public string ConsultantName { get; set; } = string.Empty;

        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();

    }
}
