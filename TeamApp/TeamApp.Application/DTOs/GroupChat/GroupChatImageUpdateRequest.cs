using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.GroupChat
{
    public class GroupChatImageUpdateRequest
    {
        public string GroupChatId { get; set; }
        public string ImageUrl { get; set; }
    }
}
