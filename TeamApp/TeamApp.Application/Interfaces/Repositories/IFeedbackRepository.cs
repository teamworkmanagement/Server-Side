using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Feedback;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IFeedbackRepository
    {
        Task<string> AddFeedback(FeedbackRequest feedbackRequest);
        Task<List<FeedbackResponse>> GetFeedbacks();
        Task<bool> MakeAsSeen(List<string> feedbackIds);
        Task<bool> RemoveFeedbacks(List<string> feedbackIds);
    }
}
