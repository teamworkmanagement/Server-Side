using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.File
{
    public class FileResponse
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public string FileUserId { get; set; }
        public string FileUserName { get; set; }
        public string FileTeamId { get; set; }
        public double FileSize { get; set; }
        public string UserImage { get; set; }
        public DateTime? FileUploadTime { get; set; }
    }
}
