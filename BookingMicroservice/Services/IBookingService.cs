using BookingMicroservice.Models;
using System.Security.Cryptography;

namespace BookingMicroservice.Services
{
    public interface IBookingService
    {
        public Task<bool> CreateBooking(BookingModel bookingModel);
    }
}
