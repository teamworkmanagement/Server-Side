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
    }
}
