using System;
using System.Collections.Generic;

namespace ConsultantCalendarMicroservice.Entities;

public partial class Patient
{
    public int Id { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public string? Postcode { get; set; }
}
