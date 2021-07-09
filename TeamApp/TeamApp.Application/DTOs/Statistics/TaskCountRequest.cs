using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class TaskStatRequest
    {
        public string OwnerType { get; set; } //team/personal
        public string StatusType { get; set; } //todo, deadline
        public string UserId { get; set; }
    }
}
