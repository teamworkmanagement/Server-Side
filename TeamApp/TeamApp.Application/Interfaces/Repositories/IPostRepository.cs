using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task<PagedResponse<PostResponse>> GetPaging(RequestParameter parameter);
        Task<PagedResponse<PostResponse>> GetPostPagingUser(PostRequestParameter parameter);
        Task<PagedResponse<PostResponse>> GetPostPagingTeam(PostRequestParameter parameter);
        Task<PostResponse> AddPost(PostRequest postReq);
        Task<bool> UpdatePost(string postId, PostRequest postReq);
        Task<bool> DeletePost(string postId);
        Task<string> AddReact(ReactModel react);
        Task<bool> DeleteReact(ReactModel react);
        Task<List<UserResponse>> SearchUser(string userId, string keyWord);
    }
}
