using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.File;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IFileRepository
    {
        Task<FileResponse> GetById(string fileId);
        Task<string> AddFile(string taskId, FileRequest fileReq);
    }
}
