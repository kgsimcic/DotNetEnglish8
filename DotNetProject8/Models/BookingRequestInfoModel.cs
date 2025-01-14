using DotNetProject8.ViewModels;

namespace DotNetProject8.Models
{
    public class BookingRequestInfoModel
    {
        public DateTime Date { get; set; }
        public ConsultantViewModel Consultant { get; set; } = new ConsultantViewModel();
    }
}
