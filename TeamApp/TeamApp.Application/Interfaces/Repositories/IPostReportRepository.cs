using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Post;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IPostReportRepository
    {
        Task<string> AddReport(CreateReportRequest feedbackRequest);
        Task<List<PostReportResponse>> GetReports();
        Task<bool> RemoveFromReport(List<string> reportIds);
        Task<bool> ChangePostStatusAccept(List<string> postIds);
        Task<bool> ChangePostStatusDeny(List<string> postIds);
    }
}
