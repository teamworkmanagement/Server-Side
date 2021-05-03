using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Team
{
    public class JoinTeamRequest
    {
        public string UserId { get; set; }
        public string TeamCode { get; set; }
    }
}
