using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Comment;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using TeamApp.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class CommentReportRepository : ICommentReportRepository
    {
        private readonly TeamAppContext _dbContext;
        public CommentReportRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddReport(CreateCommentReport commentRequest)
        {
            var report = await (from pr in _dbContext.CommentReport
                                where pr.CommentReportCommentId == commentRequest.CommentId
                                && pr.CommentReportUserId == commentRequest.UserId
                                select pr).FirstOrDefaultAsync();
            if (report != null)
            {
                throw new AlreadyExistsException("Already report");
            }

            else
            {
                report = new CommentReport
                {
                    CommentReportId = Guid.NewGuid().ToString(),
                    CommentReportCommentId = commentRequest.CommentId,
                    CommentReportUserId = commentRequest.UserId,
                };
                await _dbContext.AddAsync(report);
            }

            await _dbContext.SaveChangesAsync();

            return report.CommentReportId;
        }

        public async Task<bool> ChangeCommentStatusAccept(List<string> commentsIds)
        {
            var comments = await _dbContext.Comment.Where(c => commentsIds.Contains(c.CommentId)
              && c.CommentIsDeleted == true).ToListAsync();

            var commentReports = await _dbContext.CommentReport.Where(c => commentsIds.Contains(c.CommentReportCommentId))
                .ToListAsync();

            await _dbContext.BulkDeleteAsync(commentReports);

            foreach (var c in comments)
            {
                c.CommentIsDeleted = false;
            }

            await _dbContext.BulkUpdateAsync(comments);
            return true;
        }

        public async Task<bool> ChangeCommentStatusDeny(List<string> commentIds)
        {
            var comments = await _dbContext.Comment.Where(c => commentIds.Contains(c.CommentId)
              && c.CommentIsDeleted == false).ToListAsync();

            var commentReports = await _dbContext.CommentReport.Where(c => commentIds.Contains(c.CommentReportCommentId))
                .ToListAsync();

            await _dbContext.BulkDeleteAsync(commentReports);

            foreach (var c in comments)
            {
                c.CommentIsDeleted = true;
            }

            await _dbContext.BulkUpdateAsync(comments);

            return true;
        }

        public async Task<List<CommentReportResponse>> GetReports()
        {
            var comments = await (from cr in _dbContext.CommentReport.AsNoTracking()
                                  select cr.CommentReportCommentId).Distinct().ToListAsync();

            var responses = new List<CommentReportResponse>();
            foreach (var c in comments)
            {
                var data = await (from comment in _dbContext.Comment.AsNoTracking()
                                  join u in _dbContext.User.AsNoTracking() on comment.CommentUserId equals u.Id
                                  where comment.CommentId == c
                                  select new CommentReportResponse
                                  {
                                      CommentId = c,
                                      Content = comment.CommentContent,
                                      UserAvatar = string.IsNullOrEmpty(u.ImageUrl) ? $"https://ui-avatars.com/api/?name={u.FullName}" : u.ImageUrl,
                                      UserName = u.FullName,
                                      ReportCounts = comment.CommentReports.Count,
                                      CreatedDate = comment.CommentCreatedAt.FormatTime(),
                                  }).FirstOrDefaultAsync();

                responses.Add(data);
            }

            responses = responses.OrderByDescending(res => res.ReportCounts).ToList();

            return responses;
        }

        public async Task<bool> RemoveFromReport(List<string> commentsIds)
        {
            var commentsReports = await _dbContext.CommentReport.Where(cr => commentsIds.Contains(cr.CommentReportCommentId)).ToListAsync();

            await _dbContext.BulkDeleteAsync(commentsReports);
            return true;
        }
    }
}
