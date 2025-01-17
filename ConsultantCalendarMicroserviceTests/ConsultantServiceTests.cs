using Moq;
using ConsultantCalendarMicroservice.Entities;
using ConsultantCalendarMicroservice.Services;
using MockQueryable.Moq;

namespace ConsultantCalendarMicroserviceTests
{

    public class ConsultantServiceTests
    {
        [Fact]
        public async Task GetConsultantsShouldReturnConsultants()
        {
            var data = new List<Consultant>
            {
                new () {
                    Id = 1,
                    Fname = "Marshall",
                    Lname = "Mathers",
                    Speciality = "Rap Battling"
                },
                new () {
                    Id = 2,
                    Fname = "Dr.",
                    Lname = "Dre",
                    Speciality = "Production"
                }
            };

            var mockSet = data.AsQueryable().BuildMockDbSet();

            var mockContext = new Mock<ConsultantCalendarDbContext>();
            mockContext.Setup(m => m.Consultants).Returns(mockSet.Object);

            var service = new ConsultantService(mockContext.Object);
            var consultants = await service.GetConsultants();

            Assert.Equal(2, consultants.Count());
            Assert.Equal("Dre", consultants.ElementAt(1).Lname);
        }
    }
}
