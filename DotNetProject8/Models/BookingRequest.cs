namespace DotNetProject8.Models
{
    public class BookingRequest
    {
        public DateTime Date { get; set; }
        public ConsultantModel Consultant { get; set; } = new ConsultantModel();
    }
}
