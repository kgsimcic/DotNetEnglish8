using ConsultantCalendarMicroservice.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ConsultantCalendarMicroservice.Models
{
    public class ConsultantModel
    {
        [Key]
        public int Id { get; set; }
        public string Fname { get; set; } = string.Empty;
        public string Lname { get; set; } = string.Empty;
        public string Speciality { get; set; } = string.Empty;
    }

    public class ConsultantModelList
    {
        public List<ConsultantCalendar> ConsultantCalendars { get; set; } = new List<ConsultantCalendar>();
        public List<Consultant> Consultants { get; set; } = new List<Consultant>();
        public int SelectedConsultantId { get; set; } = int.MaxValue;
        public SelectList ConsultantsList { get; set; }
    }
}
