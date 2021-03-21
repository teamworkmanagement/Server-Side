using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Comment;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        public Task<string> AddComment(CommentRequest cmtReq)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteComment(string cmtId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CommentResponse>> GetAllByTeamId(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CommentResponse>> GetAllByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CommentResponse>> GetAllByUserTeamId(string userId, string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<CommentResponse>> GetPaging(RequestParameter parameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateComment(string cmtId, CommentRequest cmtReq)
        {
            throw new NotImplementedException();
        }
    }
}
