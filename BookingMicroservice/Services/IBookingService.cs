using BookingMicroservice.Models;
using System.Security.Cryptography;

namespace BookingMicroservice.Services
{
    public interface IBookingService
    {
        public Task<int> CreateBooking(BookingModel bookingModel);
        // public Task<IEnumerable<AppointmentDetails>> GetBookings(int ConsultantId, int month);
    }
}
