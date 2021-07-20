using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Feedback;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly TeamAppContext _dbContext;
        public FeedbackRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddFeedback(FeedbackRequest feedbackRequest)
        {
            var entity = new Feedback
            {
                FeedbackId = Guid.NewGuid().ToString(),
                FeedbackContent = feedbackRequest.FeedbackContent,
                FeedbackCreatedAt = DateTime.UtcNow,
                UserFeedbackId = feedbackRequest.UserFeedbackId,
            };

            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.UserFeedbackId;
        }

        public async Task<List<FeedbackResponse>> GetFeedbacks()
        {
            var feedbacks = await (from f in _dbContext.Feedback.AsNoTracking()
                                   join u in _dbContext.User.AsNoTracking() on f.UserFeedbackId equals u.Id
                                   orderby f.FeedbackCreatedAt descending
                                   select new { f.FeedbackId, f.FeedbackContent, u.FullName, u.ImageUrl, f.FeedbackCreatedAt, f.IsSeen }).ToListAsync();

            var response = feedbacks.Select(f => new FeedbackResponse
            {
                Id = f.FeedbackId,
                Content = f.FeedbackContent,
                UserName = f.FullName,
                UserAvatar = string.IsNullOrEmpty(f.ImageUrl) ? $"https://ui-avatars.com/api/?name={f.FullName}" : f.ImageUrl,
                CreatedDate = f.FeedbackCreatedAt.FormatTime(),
                Status = f.IsSeen == true ? 1 : 0,
            }).ToList();

            return response;
        }

        public async Task<bool> MakeAsSeen(List<string> feedbackIds)
        {
            var feedbacks = await _dbContext.Feedback.Where(f => feedbackIds.Contains(f.FeedbackId)).ToListAsync();
            foreach (var f in feedbacks)
            {
                f.IsSeen = true;
            }

            await _dbContext.BulkUpdateAsync(feedbacks);

            return true;
        }

        public async Task<bool> RemoveFeedbacks(List<string> feedbackIds)
        {
            var feedbacks = await _dbContext.Feedback.Where(f => feedbackIds.Contains(f.FeedbackId)).ToListAsync();
            await _dbContext.BulkDeleteAsync(feedbacks);
            return true;
        }
    }
}
