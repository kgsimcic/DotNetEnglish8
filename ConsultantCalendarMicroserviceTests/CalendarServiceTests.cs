using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Models;
using ConsultantCalendarMicroservice.Services;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsultantCalendarMicroserviceTests
{
    public class CalendarServiceTests
    {
        private List<ConsultantCalendar> _data;
        public CalendarServiceTests() {
            _data = new ()
            {
                new () {
                    Id = 1,
                    ConsultantId = 1,
                    Date = DateTime.Parse("2025-03-01"),
                    Available = true
                },
                new () {
                    Id = 2,
                    ConsultantId = 2,
                    Date = DateTime.Parse("2025-04-01"),
                    Available = true
                }
            };
        }

        [Fact]
        public async Task GetConsultantCalendarsShouldReturnConsultantCalendars()
        {
            var mockSet = _data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<ConsultantCalendarDbContext>();
            mockContext.Setup(m => m.ConsultantCalendars).Returns(mockSet.Object);

            var service = new CalendarService(mockContext.Object);
            var calendars = await service.GetConsultantCalendars(3);

            Assert.Single(calendars);
            Assert.Equal(1, calendars.First().ConsultantId);
        }

        [Fact]
        public async Task GetConsultantCalendarsShouldReturnEmpty()
        {
            var mockSet = _data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<ConsultantCalendarDbContext>();
            mockContext.Setup(m => m.ConsultantCalendars).Returns(mockSet.Object);

            var service = new CalendarService(mockContext.Object);
            var calendars = await service.GetConsultantCalendars(5);

            Assert.Empty(calendars);
        }
    }
}