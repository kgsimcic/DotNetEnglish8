using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DotNetProject8.Models
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
        public List<ConsultantCalendarModel>? ConsultantCalendars { get; set; }
        public List<ConsultantModel>? Consultants { get; set; }
        public int SelectedConsultantId { get; set; } = int.MaxValue;
        public SelectList? ConsultantsList { get; set; }
    }
}
