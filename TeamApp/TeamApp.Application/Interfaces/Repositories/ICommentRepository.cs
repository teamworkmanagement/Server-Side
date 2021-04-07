using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Comment;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;


namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<List<CommentResponse>> GetAllByPostId(string postId);
        Task<List<CommentResponse>> GetAllByUserId(string userId);
        Task<List<CommentResponse>> GetAllByTeamId(string teamId);
        Task<List<CommentResponse>> GetAllByUserTeamId(string userId, string teamId);
        Task<PagedResponse<CommentResponse>> GetPaging(CommentRequestParameter parameter);
        Task<string> AddComment(CommentRequest cmtReq);
        Task<bool> UpdateComment(string cmtId, CommentRequest cmtReq);
        Task<bool> DeleteComment(string cmtId);
    }
}
