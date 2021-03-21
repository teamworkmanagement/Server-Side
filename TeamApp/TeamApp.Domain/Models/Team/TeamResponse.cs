using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Models.Team
{
    public class TeamResponse
    {
        public string TeamId { get; set; }
        public string TeamLeaderId { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public DateTime? TeamCreatedAt { get; set; }
        public string TeamCode { get; set; }
        public bool? TeamIsDeleted { get; set; }
    }
}
