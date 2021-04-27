using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.File;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IFileRepository
    {
        Task<FileResponse> GetById(string fileId);
        Task<string> AddFileTask(string taskId, FileRequest fileReq);
        Task<string> AddFile(FileRequest fileReq);
        Task<bool> UpdateFile(string fileId);
        Task<PagedResponse<FileResponse>> GetByBelong(FileRequestParameter parameter);
    }
}
