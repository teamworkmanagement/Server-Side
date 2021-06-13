using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Statistics
{
    public class StatisticsRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string UserId { get; set; }
        public string BoardId { get; set; }
        public string Filter { get; set; }
    }
}
