namespace ConsultantCalendarMicroservice.Models
{
    public class AppointmentModel
    {
        public int Id { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public int? ConsultantId { get; set; }

        public int? PatientId { get; set; }
    }
}
