using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNetProject8.Models
{
    public class BookingModel
    {
        public PatientDetails Patient { get; set; } = new PatientDetails();
        public AppointmentDetails Appointment { get; set; } = new AppointmentDetails();
    }

    public class AppointmentDetails
    {
        public string ConnectionId { get; set; } = null!;
        public DateTime AppointmentDate { get; set; } = new DateTime();
        public DateTime AppointmentTime { get; set; } = new DateTime();
        public int ConsultantId { get; set; } = new int();
    }

    public class PatientDetails
    {
        public string PatientFName { get; set; } = String.Empty;
        public string PatientLName { get; set; } = String.Empty;
        public string AddressLine1 { get; set; } = String.Empty;
        public string? AddressLine2 { get; set; }
        public string? Region { get; set; }
        public string City { get; set; } = String.Empty;
        public string Postcode { get; set; } = String.Empty;
        public string ContactNumber { get; set; } = String.Empty;
    }
}
