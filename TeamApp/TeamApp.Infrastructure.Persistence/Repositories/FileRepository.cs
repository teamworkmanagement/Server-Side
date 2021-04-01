using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.File;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly TeamAppContext _dbContext;

        public FileRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddFile(string taskId, FileRequest fileReq)
        {
            var entityTask = await _dbContext.Task.FindAsync(taskId);
            if (entityTask == null)
                return string.Empty;

            var entity = new File
            {
                FileId = Guid.NewGuid().ToString(),
                FileName = fileReq.FileName,
                FileType = fileReq.FileType,
                FileUrl = fileReq.FileUrl,
            };

            await _dbContext.File.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.FileId;
        }

        public async Task<FileResponse> GetById(string fileId)
        {
            var entity = await _dbContext.File.FindAsync(fileId);
            if (entity == null)
                return null;

            return new FileResponse
            {
                FileId = entity.FileId,
                FileName = entity.FileName,
                FileUrl = entity.FileUrl,
                FileType = entity.FileType,
            };
        }
    }
}
