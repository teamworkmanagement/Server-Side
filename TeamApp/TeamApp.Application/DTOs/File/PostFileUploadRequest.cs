using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.File
{
    public class PostFileUploadRequest
    {
        public string PostId { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
