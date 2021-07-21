using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Comment;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ICommentReportRepository
    {
        Task<string> AddReport(CreateCommentReport feedbackRequest);
        Task<List<CommentReportResponse>> GetReports();
        Task<bool> RemoveFromReport(List<string> commentsIds);
        Task<bool> ChangeCommentStatusAccept(List<string> commentsIds);
        Task<bool> ChangeCommentStatusDeny(List<string> commentIds);
    }
}
