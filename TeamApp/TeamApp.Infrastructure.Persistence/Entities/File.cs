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
        public string FileUserUploadId { get; set; }
        public string FileUserOwnerId { get; set; }
        public string FileTeamOwnerId { get; set; }
        public string FileTaskOwnerId { get; set; }
        public string FilePostOwnerId { get; set; }
        public double FileSize { get; set; }
        public DateTime? FileUploadTime { get; set; }
        public virtual User UserUpload { get; set; }
        public virtual User UserOwner { get; set; }
        public virtual Team TeamOwner { get; set; }
        public virtual Task TaskOwner { get; set; }
        public virtual Post PostOwner { get; set; }
    }
}
