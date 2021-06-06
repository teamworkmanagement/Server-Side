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
                FileUserUploadId = fileReq.FileUserUploadId,
                FileUserOwnerId = fileReq.FileUserOwnerId,
                FileTeamOwnerId = fileReq.FileTeamOwnerId,
                FileTaskOwnerId = fileReq.FileTaskOwnerId,
                FilePostOwnerId = fileReq.FilePostOwnerId,
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
                    FileUserUploadId = fileReq.FileUserUploadId,
                    FileUserOwnerId = fileReq.FileUserOwnerId,
                    FileTeamOwnerId = fileReq.FileTeamOwnerId,
                    FileTaskOwnerId = fileReq.FileTaskOwnerId,
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
                FileUserUploadId = fileReq.FileUserUploadId,
                FileUserOwnerId = fileReq.FileUserOwnerId,
                FileTeamOwnerId = fileReq.FileTeamOwnerId,
                FileTaskOwnerId = fileReq.FileTaskOwnerId,
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
                FileUserUploadId = entity.FileUserUploadId,
                FileUserOwnerId = entity.FileUserOwnerId,
                FileTeamOwnerId = entity.FileTeamOwnerId,
                FileTaskOwnerId = entity.FileTaskOwnerId,
                FilePostOwnerId = entity.FilePostOwnerId,
                FileUploadTime = entity.FileUploadTime,
            };
        }


        public async Task<PagedResponse<FileResponse>> GetByBelong(FileRequestParameter param)
        {
            List<FileResponse> responses = new List<FileResponse>();
            int count = 0;
            switch (param.OwnerType)
            {
                case "user":
                    var query = from f in _dbContext.File.AsNoTracking()
                                join u in _dbContext.User.AsNoTracking() on f.FileUserUploadId equals u.Id
                                orderby f.FileUploadTime descending
                                where f.FileUserOwnerId == param.OwnerId
                                select new { f, u.FullName, u.ImageUrl };

                    var results = await query
                   .Select(p => new
                   {
                       p,
                       TotalCount = query.Count()
                   }).Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize).ToArrayAsync();

                    count = results.FirstOrDefault()?.TotalCount ?? await query.CountAsync();
                    var files = results.Select(r => r.p).ToArray();


                    responses = files.Select(entity => new FileResponse
                    {
                        FileId = entity.f.FileId,
                        FileName = entity.f.FileName,
                        FileUrl = entity.f.FileUrl,
                        FileType = entity.f.FileType,
                        FileSize = entity.f.FileSize,
                        FileUserUploadId = entity.f.FileUserUploadId,
                        FileUserOwnerId = entity.f.FileUserOwnerId,
                        FileTeamOwnerId = entity.f.FileTeamOwnerId,
                        FileTaskOwnerId = entity.f.FileTaskOwnerId,
                        FilePostOwnerId = entity.f.FilePostOwnerId,
                        FileUploadTime = entity.f.FileUploadTime.FormatTime(),
                        FileUserUploadName = entity.FullName,
                        UserImage = entity.ImageUrl,
                    }).ToList();
                    break;
                case "team":

                    var team = await _dbContext.Team.FindAsync(param.OwnerId);
                    if (team == null)
                        throw new KeyNotFoundException("Team not found");

                    query = from f in _dbContext.File.AsNoTracking()
                            join u in _dbContext.User.AsNoTracking() on f.FileUserUploadId equals u.Id
                            orderby f.FileUploadTime descending
                            where f.FileTeamOwnerId == param.OwnerId
                            select new { f, u.FullName, u.ImageUrl };
                    results = await query
                   .Select(p => new
                   {
                       p,
                       TotalCount = query.Count()
                   }).Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize).ToArrayAsync();

                    count = results.FirstOrDefault()?.TotalCount ?? await query.CountAsync();
                    files = results.Select(r => r.p).ToArray();


                    responses = files.Select(entity => new FileResponse
                    {
                        FileId = entity.f.FileId,
                        FileName = entity.f.FileName,
                        FileUrl = entity.f.FileUrl,
                        FileType = entity.f.FileType,
                        FileSize = entity.f.FileSize,
                        FileUserUploadId = entity.f.FileUserUploadId,
                        FileUserOwnerId = entity.f.FileUserOwnerId,
                        FileTeamOwnerId = entity.f.FileTeamOwnerId,
                        FileTaskOwnerId = entity.f.FileTaskOwnerId,
                        FilePostOwnerId = entity.f.FilePostOwnerId,
                        FileUploadTime = entity.f.FileUploadTime.FormatTime(),
                        FileUserUploadName = entity.FullName,
                        UserImage = entity.ImageUrl,
                    }).ToList();
                    break;
                case "task":
                    query = from f in _dbContext.File.AsNoTracking()
                            join u in _dbContext.User.AsNoTracking() on f.FileUserUploadId equals u.Id
                            orderby f.FileUploadTime descending
                            where f.FileTaskOwnerId == param.OwnerId
                            select new { f, u.FullName, u.ImageUrl };
                    results = await query
                   .Select(p => new
                   {
                       p,
                       TotalCount = query.Count()
                   }).Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize).ToArrayAsync();

                    count = results.FirstOrDefault()?.TotalCount ?? await query.CountAsync();
                    files = results.Select(r => r.p).ToArray();


                    responses = files.Select(entity => new FileResponse
                    {
                        FileId = entity.f.FileId,
                        FileName = entity.f.FileName,
                        FileUrl = entity.f.FileUrl,
                        FileType = entity.f.FileType,
                        FileSize = entity.f.FileSize,
                        FileUserUploadId = entity.f.FileUserUploadId,
                        FileUserOwnerId = entity.f.FileUserOwnerId,
                        FileTeamOwnerId = entity.f.FileTeamOwnerId,
                        FileTaskOwnerId = entity.f.FileTaskOwnerId,
                        FilePostOwnerId = entity.f.FilePostOwnerId,
                        FileUploadTime = entity.f.FileUploadTime.FormatTime(),
                        FileUserUploadName = entity.FullName,
                        UserImage = entity.ImageUrl,
                    }).ToList();
                    break;
                case "post":
                    query = from f in _dbContext.File.AsNoTracking()
                            join u in _dbContext.User.AsNoTracking() on f.FileUserUploadId equals u.Id
                            orderby f.FileUploadTime descending
                            where f.FilePostOwnerId == param.OwnerId
                            select new { f, u.FullName, u.ImageUrl };
                    results = await query
                   .Select(p => new
                   {
                       p,
                       TotalCount = query.Count()
                   }).Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize).ToArrayAsync();

                    count = results.FirstOrDefault()?.TotalCount ?? await query.CountAsync();
                    files = results.Select(r => r.p).ToArray();


                    responses = files.Select(entity => new FileResponse
                    {
                        FileId = entity.f.FileId,
                        FileName = entity.f.FileName,
                        FileUrl = entity.f.FileUrl,
                        FileType = entity.f.FileType,
                        FileSize = entity.f.FileSize,
                        FileUserUploadId = entity.f.FileUserUploadId,
                        FileUserOwnerId = entity.f.FileUserOwnerId,
                        FileTeamOwnerId = entity.f.FileTeamOwnerId,
                        FileTaskOwnerId = entity.f.FileTaskOwnerId,
                        FilePostOwnerId = entity.f.FilePostOwnerId,
                        FileUploadTime = entity.f.FileUploadTime.FormatTime(),
                        FileUserUploadName = entity.FullName,
                        UserImage = entity.ImageUrl,
                    }).ToList();
                    break;
                default:
                    break;
            }


            return new PagedResponse<FileResponse>(responses, param.PageSize, count, param.PageNumber);
        }

        public Task<bool> UpdateFile(string fileId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FileResponse>> GetAllByTask(string taskId)
        {
            var query = from t in _dbContext.Task.AsNoTracking()
                        join f in _dbContext.File.AsNoTracking() on t.TaskId equals f.FileTaskOwnerId
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
                FileUserUploadId = entity.FileUserUploadId,
                FileTaskOwnerId = entity.FileTaskOwnerId,
                FileUploadTime = entity.FileUploadTime.FormatTime(),
            }).ToListAsync();

            return outPut;
        }

        public async Task<bool> UploadImageForPost(PostFileUploadRequest postFileUploadRequest)
        {
            List<File> files = new List<File>();
            foreach (var obj in postFileUploadRequest.ImageUrls)
            {
                files.Add(new File
                {
                    FileId = Guid.NewGuid().ToString(),
                    FileUrl = obj.Link,
                    FilePostOwnerId = postFileUploadRequest.PostId,
                    FileUploadTime = DateTime.UtcNow,
                });
            }

            await _dbContext.BulkInsertAsync(files);

            return true;
        }
    }
}
