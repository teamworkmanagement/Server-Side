﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Models.Post;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task<List<PostResponse>> GetAllByUserId(string userId);
        Task<List<PostResponse>> GetAllByTeamId(string teamId);
        Task<List<PostResponse>> GetAllByUserTeamId(string userId, string teamId);
        Task<PagedResponse<PostResponse>> GetPaging(RequestParameter parameter);
        Task<string> AddPost(PostRequest postReq);
        Task<bool> UpdatePost(string postId, PostRequest postReq);
        Task<bool> DeletePost(string postId);
    }
}