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
        public string FileUserUploadId { get; set; }
        public string FileUserOwnerId { get; set; }
        public string FileTeamOwnerId { get; set; }
        public string FileTaskOwnerId { get; set; }
        public string FilePostOwnerId { get; set; }
        public double FileSize { get; set; }
    }
}
