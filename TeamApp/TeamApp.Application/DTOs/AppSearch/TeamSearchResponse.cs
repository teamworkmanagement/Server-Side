using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.AppSearch
{
    public class TeamSearchResponse
    {
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public string TeamCode { get; set; }
        public string TeamImage { get; set; }
        public string Link { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
