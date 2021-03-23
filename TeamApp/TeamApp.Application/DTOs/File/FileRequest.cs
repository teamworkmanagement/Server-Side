using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.File
{
    public class FileRequest
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }
}
