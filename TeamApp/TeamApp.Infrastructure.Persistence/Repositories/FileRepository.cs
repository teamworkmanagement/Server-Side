using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.File;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class FileRepository : IFileRepository
    {
        public Task<string> AddFile(string taskId, FileRequest fileReq)
        {
            throw new NotImplementedException();
        }

        public Task<FileResponse> GetById(string fileId)
        {
            throw new NotImplementedException();
        }
    }
}
