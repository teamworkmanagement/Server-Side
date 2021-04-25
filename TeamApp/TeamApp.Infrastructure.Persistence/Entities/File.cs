using System;
using System.Collections.Generic;

namespace TeamApp.Infrastructure.Persistence.Entities
{
    public partial class File
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public string FileBelongedId { get; set; }
        public string FileUserId { get; set; }
        public double FileSize { get; set; }
        public DateTime? FileUploadTime { get; set; }
        public virtual User User { get; set; }
    }
}
