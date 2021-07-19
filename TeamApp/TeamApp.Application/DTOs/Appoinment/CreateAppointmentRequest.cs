using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Appoinment
{
    public class CreateAppointmentRequest
    {
        public string UserCreateId { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string TeamId { get; set; }
    }
}
