using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Team
{
    public class TeamUpdateRequest
    {
        public string TeamId { get; set; }
        public string TeamLeaderId { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public string TeamImageUrl { get; set; }
    }
}
