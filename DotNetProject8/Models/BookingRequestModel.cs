using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetProject8.Models
{

    public class BookingRequestModel
    {
        public PatientRequestDetails Patient { get; set; } = new PatientRequestDetails();
        public ConsultantRequestDetails Consultant { get; set; } = new ConsultantRequestDetails();
        public AppointmentRequestDetails Appointment { get; set; } = new AppointmentRequestDetails();
    }

    [BindProperties]
    public class AppointmentRequestDetails
    {
        public long AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; } = new DateTime();
        public SelectList? AppointmentTimes { get; set; }
        public DateTime SelectedAppointmentTime { get; set; }
    }

    [BindProperties]
    public class ConsultantRequestDetails
    {
        public int ConsultantId { get; set; }
        public string? ConsultantName { get; set; }
        public string? ConsultantSpeciality { get; set; }
    }

    [BindProperties]
    public class PatientRequestDetails
    {
        public string? PatientFName { get; set; }
        public string? PatientLName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Postcode { get; set; }
        public string? ContactNumber { get; set; }
    }
}
