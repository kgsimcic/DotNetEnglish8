﻿using BookingMicroservice.Models;
using System.Security.Cryptography;

namespace BookingMicroservice.Services
{
    public interface IBookingService
    {
        public Task<IEnumerable<AppointmentDetails>> GetBookings(DateTime selectedDate);
        public Task<string> ProcessBookingAsync(BookingModel bookingModel);
        public Task<int> GetAppointmentCountAsync(string cacheKey, DateTime startDateTime);
    }
}
