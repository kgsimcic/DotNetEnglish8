using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DotNetProject8.ViewModels
{
    public class ConsultantViewModel
    {
        public int Id { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Speciality { get; set; }
    }

    public class ConsultantViewModelList
    {
        public List<ConsultantCalendarViewModel>? ConsultantCalendars { get; set; }
        public List<ConsultantViewModel>? Consultants { get; set; }
        public int SelectedConsultantId { get; set; } = int.MaxValue;
        public SelectList? ConsultantsList { get; set; }
    }
}
