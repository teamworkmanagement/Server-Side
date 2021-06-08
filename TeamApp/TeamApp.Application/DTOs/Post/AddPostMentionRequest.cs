using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Post
{
    public class AddPostMentionRequest
    {
        public string ActionUserId { get; set; }
        public List<string> UserIds { get; set; }
        public string PostId { get; set; }
    }
}
