using System;
using System.Collections.Generic;

namespace ConsultantCalendarMicroservice.Entities;

public partial class Consultant
{
    public int Id { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Speciality { get; set; }
}
