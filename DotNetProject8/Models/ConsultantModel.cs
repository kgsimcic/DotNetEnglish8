using ConsultantCalendarMicroservice.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ConsultantCalendarMicroservice.Models
{
    public class ConsultantModel
    {
        [Key]
        public int Id { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Speciality { get; set; }
    }

    public class ConsultantModelList
    {
        public List<ConsultantCalendar>? ConsultantCalendars { get; set; }
        public List<ConsultantModel>? Consultants { get; set; }
        public int SelectedConsultantId { get; set; } = int.MaxValue;
        public SelectList? ConsultantsList { get; set; }
    }
}
