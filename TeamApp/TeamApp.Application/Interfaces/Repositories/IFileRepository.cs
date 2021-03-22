using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.File;
using TeamApp.Domain.Models.TaskVersion;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IFileRepository
    {
        Task<FileResponse> GetById(string fileId);
        Task<string> AddFile(string taskId, FileRequest fileReq);
    }
}
