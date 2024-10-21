using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingMicroservice.Entities;

public partial class Appointment
{
    [Key]
    public int Id { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public int ConsultantId { get; set; }

    public int PatientId { get; set; }
}
