using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.AppSearch
{
    public class TaskSearchResponse
    {
        public string TaskId { get; set; }
        public string Link { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
