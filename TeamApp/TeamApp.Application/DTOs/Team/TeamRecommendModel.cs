using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Team
{
    public class TeamRecommendModel
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupAvatar { get; set; }
        public int GroupNewPostCount { get; set; }
        public int GroupMemberCount { get; set; }
    }
}
