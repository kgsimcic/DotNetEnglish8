using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetProject8.Models
{
    public class BookingModel
    {
        public PatientDetails Patient { get; set; } = new PatientDetails();
        public ConsultantDetails Consultant { get; set; } = new ConsultantDetails();
        public AppointmentDetails Appointment { get; set; } = new AppointmentDetails();
    }

    public class AppointmentDetails
    {
        public Guid AppointmentId { get; set; } = new Guid();
        public DateTime AppointmentDate { get; set; } = new DateTime();
        public DateTime AppointmentTime { get; set; } = new DateTime();
    }

    public class ConsultantDetails
    {
        public int ConsultantId { get; set; }
        public string ConsultantName { get; set; }
        public string ConsultantSpeciality { get; set; }
    }

    public class PatientDetails
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string ContactNumber { get; set; }
    }
}
