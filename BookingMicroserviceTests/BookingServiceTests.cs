using BookingMicroservice.Entities;
using BookingMicroservice.Models;
using BookingMicroservice.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using System;
using System.Drawing;

namespace BookingMicroserviceTests
{
    public class BookingServiceTests
    {
        private readonly List<Appointment> _data;
        private readonly PatientDetails _patientDetails;
        public BookingServiceTests() {
            DateTime datetime1 = DateTime.ParseExact("2025-03-01 08:00", "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            DateTime datetime2 = datetime1.AddDays(1);
            _data = new() {
                new ()
                {
                     Id = 1,
                     StartDateTime = datetime1,
                     EndDateTime = datetime1.AddHours(1),
                     ConsultantId = 1,
                     PatientId = 1
                },
                new ()
                {
                     Id = 2,
                     StartDateTime = datetime2,
                     EndDateTime = datetime2.AddHours(1),
                     ConsultantId = 2,
                     PatientId = 2
                }
            };

            _patientDetails = new()
            {
                PatientFName = "Spongebob",
                PatientLName = "Squarepants",
                AddressLine1 = "1507 Pineapple Lane",
                City = "Bikini Bottom",
                Postcode = "12345"
            };
        }

        [Fact]
        public async Task GetBookingsShouldReturnBookings()
        {
            var mockSet = _data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<BookingDbContext>();
            mockContext.Setup(m => m.Appointments).Returns(mockSet.Object);

            var mockLogger = new Mock<ILogger<BookingService>>().Object;
            var mockCache = new Mock<IMemoryCache>().Object;

            var service = new BookingService(mockLogger, mockContext.Object, mockCache);
            var appointments = await service.GetBookings(DateTime.Parse("2025-03-01"));

            Assert.Single(appointments);
            Assert.Equal(1, appointments.First().ConsultantId);
        }

        [Fact]
        public async Task GetBookingsShouldReturnEmpty()
        {
            var mockSet = _data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<BookingDbContext>();
            mockContext.Setup(m => m.Appointments).Returns(mockSet.Object);

            var mockLogger = new Mock<ILogger<BookingService>>().Object;
            var mockCache = new Mock<IMemoryCache>().Object;

            var service = new BookingService(mockLogger, mockContext.Object, mockCache);
            var appointments = await service.GetBookings(DateTime.Parse("2025-04-01"));

            Assert.Empty(appointments);
        }

        [Fact]
        public async Task GetAppointmentCountShouldReturnPositiveCount()
        {
            DateTime conflictTime = DateTime.ParseExact("2025-03-01 08:00", "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            
            var mockSet = _data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<BookingDbContext>();
            mockContext.Setup(m => m.Appointments).Returns(mockSet.Object);

            var mockLogger = new Mock<ILogger<BookingService>>().Object;
            var mockCache = new Mock<IMemoryCache>().Object;

            var service = new BookingService(mockLogger, mockContext.Object, mockCache);
            int appointmentCount = await service.GetAppointmentCountAsync("", conflictTime);

            Assert.Equal(1, appointmentCount);
        }

        [Fact]
        public async Task GetAppointmentCountShouldReturnZero()
        {
            DateTime nonConflictTime = DateTime.ParseExact("2025-04-01 08:00", "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            var mockSet = _data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<BookingDbContext>();
            mockContext.Setup(m => m.Appointments).Returns(mockSet.Object);

            var mockLogger = new Mock<ILogger<BookingService>>().Object;
            var mockCache = new Mock<IMemoryCache>().Object;

            var service = new BookingService(mockLogger, mockContext.Object, mockCache);
            int appointmentCount = await service.GetAppointmentCountAsync("", nonConflictTime);

            Assert.Equal(0, appointmentCount);
        }

        [Fact]
        public async Task ProcessBookingAsyncShouldReturnFailed()
        {
            DateTime conflictTime = DateTime.ParseExact("2025-03-01 08:00", "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            AppointmentDetails conflictingAppointment = new()
            {
                AppointmentDate = conflictTime,
                AppointmentTime = conflictTime,
                ConsultantId = 1
            };

            BookingModel conflictingModel = new()
            {
                Appointment = conflictingAppointment,
                Patient = _patientDetails
            };

            // set up DbContext
            var mockSet = _data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<BookingDbContext>();
            mockContext.Setup(m => m.Appointments).Returns(mockSet.Object);

            var mockLogger = new Mock<ILogger<BookingService>>().Object;
            var mockCache = new Mock<IMemoryCache>();
            var mockEntry = new Mock<ICacheEntry>();

            // set up Caching responses
            var url = "fakeURL";
            var response = "json string";

            mockCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(mockEntry.Object);

            var cachedResponse = mockCache.Object.Set<string>(url, response);

            var service = new BookingService(mockLogger, mockContext.Object, mockCache.Object);
            string status = await service.ProcessBookingAsync(conflictingModel);

            Assert.Equal("Failed", status);
        }

        [Fact]
        public async Task ProcessBookingAsyncShouldReturnCompleted()
        {
            DateTime nonConflictTime = DateTime.ParseExact("2025-04-01 08:00", "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            AppointmentDetails nonConflictingAppointment = new()
            {
                AppointmentDate = nonConflictTime,
                AppointmentTime = nonConflictTime,
                ConsultantId = 1
            };

            BookingModel nonConflictingModel = new()
            {
                Appointment = nonConflictingAppointment,
                Patient = _patientDetails
            };

            var mockSet = _data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<BookingDbContext>();
            mockContext.Setup(m => m.Appointments).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default))
                        .Returns(Task.FromResult(1))
                        .Verifiable();

            var mockLogger = new Mock<ILogger<BookingService>>().Object;
            var mockCache = new Mock<IMemoryCache>();
            var mockEntry = new Mock<ICacheEntry>();

            // set up Caching responses
            var url = "fakeURL";
            var response = "json string";

            mockCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(mockEntry.Object);

            var cachedResponse = mockCache.Object.Set<string>(url, response);

            var service = new BookingService(mockLogger, mockContext.Object, mockCache.Object);
            string status = await service.ProcessBookingAsync(nonConflictingModel);

            Assert.Equal("Completed", status);
        }
    }
}