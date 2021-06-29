using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Feedback;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;

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
    }
}
