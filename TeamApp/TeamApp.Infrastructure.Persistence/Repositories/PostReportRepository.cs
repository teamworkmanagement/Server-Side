using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class PostReportRepository : IPostReportRepository
    {
        private readonly TeamAppContext _dbContext;
        public PostReportRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddReport(CreateReportRequest reportRequest)
        {
            var report = await (from pr in _dbContext.PostReport
                                where pr.PostId == reportRequest.PostId
                                select pr).FirstOrDefaultAsync();
            if (report != null)
            {
                report.ReportCount++;
                _dbContext.Update(report);
            }

            else
            {
                report = new PostReport
                {
                    Id = Guid.NewGuid().ToString(),
                    PostId = reportRequest.PostId,
                    UserReportId = reportRequest.UserReportId,
                    Status = "0",
                    ReportCount = 1,
                };
                await _dbContext.AddAsync(report);
            }

            await _dbContext.SaveChangesAsync();

            return report.Id;
        }

        public async Task<bool> ChangePostStatusAccept(List<string> postIds)
        {
            var posts = await _dbContext.Post.Where(p => postIds.Contains(p.PostId)
              && p.PostIsDeleted == true).ToListAsync();

            foreach (var p in posts)
            {
                p.PostIsDeleted = false;
            }

            var postReports = await _dbContext.PostReport.Where(pr => postIds.Contains(pr.PostId)
            ).ToListAsync();

            foreach (var pr in postReports)
            {
                pr.Status = "1";
            }

            await _dbContext.BulkUpdateAsync(posts);
            await _dbContext.BulkUpdateAsync(postReports);

            return true;
        }

        public async Task<bool> ChangePostStatusDeny(List<string> postIds)
        {
            var posts = await _dbContext.Post.Where(p => postIds.Contains(p.PostId)
              && p.PostIsDeleted == false).ToListAsync();

            foreach (var p in posts)
            {
                p.PostIsDeleted = true;
            }

            var postReports = await _dbContext.PostReport.Where(pr => postIds.Contains(pr.PostId)
            ).ToListAsync();

            foreach (var pr in postReports)
            {
                pr.Status = "2";
            }

            await _dbContext.BulkUpdateAsync(posts);
            await _dbContext.BulkUpdateAsync(postReports);

            return true;
        }

        public async Task<List<PostReportResponse>> GetReports()
        {
            var postReports = await (from pr in _dbContext.PostReport.AsNoTracking()
                                     join p in _dbContext.Post.AsNoTracking() on pr.PostId equals p.PostId
                                     join u in _dbContext.User.AsNoTracking() on p.PostUserId equals u.Id
                                     orderby pr.ReportCount descending
                                     select new { pr.Id, pr.PostId, p.PostContent, u.ImageUrl, u.FullName, p.PostCreatedAt, pr.Status })
                                   .ToListAsync();

            var response = new List<PostReportResponse>();
            foreach (var pr in postReports)
            {
                var imagesStr = await (from f in _dbContext.File.AsNoTracking()
                                       where f.FilePostOwnerId == pr.PostId
                                       select f.FileUrl).ToListAsync();

                var images = new List<object>();
                foreach (var img in imagesStr)
                {
                    images.Add(new
                    {
                        Original = img,
                    });
                }
                response.Add(new PostReportResponse
                {
                    Id = pr.Id,
                    Content = pr.PostContent,
                    UserAvatar = pr.ImageUrl,
                    UserName = pr.FullName,
                    Status = pr.Status,
                    Images = images,
                });
            }

            return response;
        }

        public async Task<bool> RemoveFromReport(List<string> reportIds)
        {
            var postReports = await _dbContext.PostReport.Where(pr => reportIds.Contains(pr.Id)).ToListAsync();

            await _dbContext.BulkDeleteAsync(postReports);
            return true;
        }
    }
}
