using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.AppSearch
{
    public class MemberChat
    {
        public string Id { get; set; }
        public string MemberName { get; set; }
        public string MemberAvatar { get; set; }
    }
    public class GroupChatSearchResponse
    {
        public string ChatId { get; set; }
        public string ChatName { get; set; }
        public string ChatImage { get; set; }
        public string Link { get; set; }
        public bool? ChatIsOfTeam { get; set; }
        public List<MemberChat> ChatListMembers { get; set; }
    }
}
