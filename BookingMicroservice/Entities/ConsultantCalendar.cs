using System;
using System.Collections.Generic;

namespace BookingMicroservice.Entities;

public partial class ConsultantCalendar
{
    public int Id { get; set; }

    public int? ConsultantId { get; set; }

    public DateTime? Date { get; set; }

    public bool? Available { get; set; }
}
