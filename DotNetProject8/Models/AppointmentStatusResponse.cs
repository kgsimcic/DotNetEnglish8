﻿namespace DotNetProject8.Models
{
    public class AppointmentStatusResponse
    {
        public long AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; } = new DateTime();
        public DateTime AppointmentTime { get; set; } = new DateTime();
        public string? Status { get; set; }

    }
}
