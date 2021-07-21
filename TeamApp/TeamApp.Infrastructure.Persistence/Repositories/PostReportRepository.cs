using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using TeamApp.Application.Utils;
using TeamApp.Application.Exceptions;

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
                                where pr.PostReportPostId == reportRequest.PostId
                                && pr.PostReportUserId == reportRequest.UserReportId
                                select pr).FirstOrDefaultAsync();
            if (report != null)
            {
                throw new AlreadyExistsException("Already report");
            }

            else
            {
                report = new PostReport
                {
                    PostReportId = Guid.NewGuid().ToString(),
                    PostReportPostId = reportRequest.PostId,
                    PostReportUserId = reportRequest.UserReportId,
                };
                await _dbContext.AddAsync(report);
            }

            await _dbContext.SaveChangesAsync();

            return report.PostReportId;
        }

        public async Task<bool> ChangePostStatusAccept(List<string> postIds)
        {
            var posts = await _dbContext.Post.Where(p => postIds.Contains(p.PostId)
              && p.PostIsDeleted == true).ToListAsync();

            var postReports = await _dbContext.PostReport.Where(p => postIds.Contains(p.PostReportPostId))
                .ToListAsync();

            await _dbContext.BulkDeleteAsync(postReports);

            foreach (var p in posts)
            {
                p.PostIsDeleted = false;
            }

            await _dbContext.BulkUpdateAsync(posts);
            return true;
        }

        public async Task<bool> ChangePostStatusDeny(List<string> postIds)
        {
            var posts = await _dbContext.Post.Where(p => postIds.Contains(p.PostId)
              && p.PostIsDeleted == false).ToListAsync();

            var postReports = await _dbContext.PostReport.Where(p => postIds.Contains(p.PostReportPostId))
                .ToListAsync();

            await _dbContext.BulkDeleteAsync(postReports);

            foreach (var p in posts)
            {
                p.PostIsDeleted = true;
            }

            await _dbContext.BulkUpdateAsync(posts);

            return true;
        }

        public async Task<List<PostReportResponse>> GetReports()
        {
            /*var postReports = await (from pr in _dbContext.PostReport.AsNoTracking()
                                     join p in _dbContext.Post.AsNoTracking() on pr.PostId equals p.PostId
                                     join u in _dbContext.User.AsNoTracking() on p.PostUserId equals u.Id
                                     orderby pr.ReportCount descending
                                     select new { pr.Id, pr.PostId, p.PostContent, pr.ReportCount, u.ImageUrl, u.FullName, p.PostCreatedAt, pr.Status })
                                   .ToListAsync();*/

            var posts = await (from pr in _dbContext.PostReport.AsNoTracking()
                               select pr.PostReportPostId).Distinct().ToListAsync();

            var responses = new List<PostReportResponse>();
            foreach (var p in posts)
            {
                var data = await (from post in _dbContext.Post.AsNoTracking()
                                  join u in _dbContext.User.AsNoTracking() on post.PostUserId equals u.Id
                                  where post.PostId == p
                                  select new PostReportResponse
                                  {
                                      PostId = p,
                                      Content = post.PostContent,
                                      UserAvatar = u.ImageUrl,
                                      UserName = u.FullName,
                                      ReportCounts = post.PostReports.Count,
                                      CreatedDate = post.PostCreatedAt,
                                  }).FirstOrDefaultAsync();

                responses.Add(data);
            }


            foreach (var pr in responses)
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

                pr.Images = images;
            }

            responses = responses.OrderByDescending(res => res.ReportCounts).ToList();

            return responses;
        }

        public async Task<bool> RemoveFromReport(List<string> postIds)
        {
            var postReports = await _dbContext.PostReport.Where(pr => postIds.Contains(pr.PostReportPostId)).ToListAsync();

            await _dbContext.BulkDeleteAsync(postReports);
            return true;
        }
    }
}
