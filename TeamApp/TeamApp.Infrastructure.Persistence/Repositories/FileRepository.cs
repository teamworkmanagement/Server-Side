using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.File;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Application.Utils;
using TeamApp.Application.Wrappers;
using TeamApp.Application.Filters;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly TeamAppContext _dbContext;

        public FileRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddFile(FileRequest fileReq)
        {
            var entity = new File
            {
                FileId = Guid.NewGuid().ToString(),
                FileName = fileReq.FileName,
                FileType = fileReq.FileType,
                FileUrl = fileReq.FileUrl,
                FileSize = fileReq.FileSize,
                FileTeamId = fileReq.TeamId,
                FileUserId = fileReq.UserId,
                FileUploadTime = DateTime.UtcNow,
            };

            await _dbContext.File.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.FileId;
        }

        public async Task<string> AddFileTask(string taskId, FileRequest fileReq)
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
                FileSize = fileReq.FileSize,
                FileTeamId = fileReq.TeamId,
                FileUserId = fileReq.UserId,
                FileUploadTime = DateTime.UtcNow,
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
                FileSize = entity.FileSize,
                FileTeamId = entity.FileTeamId,
                FileUserId = entity.FileUserId,
                FileUploadTime = entity.FileUploadTime,
            };
        }

        public async Task<PagedResponse<FileResponse>> GetByTeamId(FileRequestParameter par)
        {
            var query = from f in _dbContext.File
                        where f.FileTeamId == par.TeamId
                        select f;

            query = query.Skip((par.PageSize - 1) * par.PageNumber).Take(par.PageSize)
                .OrderByDescending(x => x.FileUploadTime);

            var outPut = await query.Select(entity => new FileResponse
            {
                FileId = entity.FileId,
                FileName = entity.FileName,
                FileUrl = entity.FileUrl,
                FileType = entity.FileType,
                FileSize = entity.FileSize,
                FileTeamId = entity.FileTeamId,
                FileUserId = entity.FileUserId,
                FileUploadTime = entity.FileUploadTime,
            }).ToListAsync();

            return new PagedResponse<FileResponse>(outPut, par.PageNumber, par.PageSize, await query.CountAsync());
        }

        public Task<bool> UpdateFile(string fileId)
        {
            throw new NotImplementedException();
        }
    }
}
