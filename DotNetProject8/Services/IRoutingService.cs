﻿using DotNetProject8.Models;

namespace DotNetProject8.Services
{
    public interface IRoutingService
    {
        public Task<List<ConsultantModel>?> GetConsultantsAsync();
        public Task<List<ConsultantCalendarModel>?> GetConsultantCalendars(int selectedMonth);

    }
}
