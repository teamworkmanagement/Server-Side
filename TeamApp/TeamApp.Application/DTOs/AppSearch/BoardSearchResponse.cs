using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.AppSearch
{
    public class BoardSearchResponse
    {
        public string BoardId { get; set; }
        public string BoardName { get; set; }
        public string BoardTeamName { get; set; }
        public string BoardTeamImage { get; set; }
        public string BoardUserId { get; set; }
        public string BoardTeamId { get; set; }
        public string Link { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
