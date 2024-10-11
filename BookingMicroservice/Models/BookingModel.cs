using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingMicroservice.Models
{
    public class BookingModel
    {
        public PatientDetails Patient { get; set; } = new PatientDetails();
        public ConsultantDetails Consultant { get; set; } = new ConsultantDetails();
        public FacilityDetails Facility { get; set; } = new FacilityDetails();
        public PaymentDetails Payment { get; set; } = new PaymentDetails();
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
        public int FacilityId { get; set; }
    }

    public class FacilityDetails
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityAddressLine1 { get; set; }
        public string FacilityAddressLine2 { get; set; }
        public string FacilityRegion { get; set; }
        public string FacilityCity { get; set; }
        public string FacilityPostcode { get; set; }
        public string FacilityContactNumber { get; set; }
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

    public class PaymentDetails
    {
        public int PaymentId { get; set; }
        public double Payment { get; set; }
    }
}
