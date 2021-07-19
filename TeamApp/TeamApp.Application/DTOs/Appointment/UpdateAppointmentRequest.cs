using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Appointment
{
    public class UpdateAppointmentRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public string UserUpdateId { get; set; }
        public string Type { get; set; }
    }
}
