namespace DotNetProject8.Models
{
    public class AppointmentModel
    {
        public DateTime AppointmentDate { get; set; } = new DateTime();
        public DateTime AppointmentTime { get; set; } = new DateTime();
        public int ConsultantId { get; set; } = new int();
    }
}
