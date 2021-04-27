using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.File
{
    public class FileRequest
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public string UserId { get; set; }
        public string FileBelongedId { get; set; }
        public double FileSize { get; set; }
    }
}
