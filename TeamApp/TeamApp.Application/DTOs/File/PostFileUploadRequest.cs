using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.DTOs.Post;

namespace TeamApp.Application.DTOs.File
{
    public class PostFileUploadRequest
    {
        public string PostId { get; set; }
        public List<PostImage> ImageUrls { get; set; }
    }
}
