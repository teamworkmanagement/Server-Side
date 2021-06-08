using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Team
{
    public class JoinTeamNotification
    {
        public string ActionUserId { get; set; }
        public string TeamId { get; set; }
        public string UserId { get; set; }
    }
}
