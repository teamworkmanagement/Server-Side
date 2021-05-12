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

        public async Task<FileResponse> AddFile(FileRequest fileReq)
        {
            var entity = new File
            {
                FileId = Guid.NewGuid().ToString(),
                FileName = fileReq.FileName,
                FileType = fileReq.FileType,
                FileUrl = fileReq.FileUrl,
                FileSize = fileReq.FileSize,
                FileBelongedId = fileReq.FileBelongedId,
                FileUserId = fileReq.UserId,
                FileUploadTime = DateTime.UtcNow,
            };

            await _dbContext.File.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
                return new FileResponse
                {
                    FileId = entity.FileId,
                    FileName = entity.FileName,
                    FileUrl = entity.FileUrl,
                    FileType = entity.FileType,
                    FileSize = entity.FileSize,
                    FileBelongedId = entity.FileBelongedId,
                    FileUserId = entity.FileUserId,
                    FileUploadTime = entity.FileUploadTime.FormatTime(),
                };
            return null;
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
                FileBelongedId = fileReq.FileBelongedId,
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
                FileBelongedId = entity.FileBelongedId,
                FileUserId = entity.FileUserId,
                FileUploadTime = entity.FileUploadTime,
            };
        }

        public async Task<PagedResponse<FileResponse>> GetByBelong(FileRequestParameter par)
        {
            var query = from f in _dbContext.File.AsNoTracking()
                        join u in _dbContext.User.AsNoTracking() on f.FileUserId equals u.Id
                        orderby f.FileUploadTime descending
                        where f.FileBelongedId == par.BelongedId
                        select new { f, u.FullName, u.ImageUrl };

            var zzz = await query.AsNoTracking().CountAsync();

            var outFile = query.Skip((par.PageNumber - 1) * par.PageSize).Take(par.PageSize);

            zzz = await outFile.CountAsync();

            var outPut = await outFile.Select(entity => new FileResponse
            {
                FileId = entity.f.FileId,
                FileName = entity.f.FileName,
                FileUrl = entity.f.FileUrl,
                FileType = entity.f.FileType,
                FileSize = entity.f.FileSize,
                FileBelongedId = entity.f.FileBelongedId,
                FileUserId = entity.f.FileUserId,
                FileUploadTime = entity.f.FileUploadTime.FormatTime(),
                FileUserName = entity.FullName,
                UserImage = entity.ImageUrl,
            }).ToListAsync();

            return new PagedResponse<FileResponse>(outPut, par.PageSize, await query.CountAsync(), par.PageNumber);
        }

        public Task<bool> UpdateFile(string fileId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FileResponse>> GetAllByTask(string taskId)
        {
            var query = from t in _dbContext.Task.AsNoTracking()
                        join f in _dbContext.File.AsNoTracking() on t.TaskId equals f.FileBelongedId
                        where t.TaskId == taskId
                        orderby f.FileUploadTime descending
                        select f;

            var outPut = await query.AsNoTracking().Select(entity => new FileResponse
            {
                FileId = entity.FileId,
                FileName = entity.FileName,
                FileUrl = entity.FileUrl,
                FileType = entity.FileType,
                FileSize = entity.FileSize,
                FileBelongedId = entity.FileBelongedId,
                FileUserId = entity.FileUserId,
                FileUploadTime = entity.FileUploadTime.FormatTime(),
            }).ToListAsync();

            return outPut;
        }

        public async Task<bool> UploadImageForPost(PostFileUploadRequest postFileUploadRequest)
        {
            List<File> files = new List<File>();
            foreach (var link in postFileUploadRequest.ImageUrls)
            {
                files.Add(new File
                {
                    FileId = Guid.NewGuid().ToString(),
                    FileUrl = link,
                    FileBelongedId = postFileUploadRequest.PostId,
                    FileUploadTime = DateTime.UtcNow,
                });
            }

            await _dbContext.BulkInsertAsync(files);

            return true;
        }
    }
}
