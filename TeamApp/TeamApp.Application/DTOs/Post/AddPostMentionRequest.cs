using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Post
{
    public class AddPostMentionRequest
    {
        public List<string> UserIds { get; set; }
        public string PostId { get; set; }
    }
}
