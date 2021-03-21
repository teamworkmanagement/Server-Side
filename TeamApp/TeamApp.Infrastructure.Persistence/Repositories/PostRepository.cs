using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Post;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class PostRepository : IPostRepository
    {
        public Task<string> AddPost(PostRequest postReq)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePost(string postId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostResponse>> GetAllByTeamId(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostResponse>> GetAllByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostResponse>> GetAllByUserTeamId(string userId, string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<PostResponse>> GetPaging(RequestParameter parameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePost(string postId, PostRequest postReq)
        {
            throw new NotImplementedException();
        }
    }
}
