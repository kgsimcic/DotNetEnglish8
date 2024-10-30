using System;
using System.Collections.Generic;

namespace BookingMicroservice.Entities;

public partial class Appointment
{
    public int Id { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int? ConsultantId { get; set; }

    public int? PatientId { get; set; }

    public virtual Patient? Patient { get; set; }
}
