using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNetProject8.Models
{
    public class BookingModel
    {
        public int SessionId { get; set; }
        public PatientDetails Patient { get; set; } = new PatientDetails();
        public AppointmentDetails Appointment { get; set; } = new AppointmentDetails();
    }

    public class AppointmentDetails
    {
        public DateTime AppointmentDate { get; set; } = new DateTime();
        public DateTime AppointmentTime { get; set; } = new DateTime();
        public int ConsultantId { get; set; } = new int();
    }

    public class PatientDetails
    {
        public string PatientFName { get; set; }
        public string PatientLName { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Region { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string ContactNumber { get; set; }
    }
}
