﻿using System.ComponentModel.DataAnnotations;

namespace ConsultantCalendarMicroservice.Models
{
    public class ConsultantCalendarModel
    {
        public int MonthId { get; set; }
        public int ConsultantId { get; set; }

        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();

    }
}
