using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.Post;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly TeamAppContext _dbContext;

        public PostRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddPost(PostRequest postReq)
        {
            var entity = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                PostUserId = postReq.PostUserId,
                PostTeamId = postReq.PostTeamId,
                PostContent = postReq.PostContent,
                PostCreatedAt = DateTime.UtcNow,
                PostCommentCount = 0,
                PostIsDeleted = false,
                PostIsPinned = false,
            };

            await _dbContext.Post.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.PostId;
        }

        public async Task<bool> DeletePost(string postId)
        {
            var entity = await _dbContext.Post.FindAsync(postId);
            if (entity == null)
                return false;

            entity.PostIsDeleted = true;
            _dbContext.Post.Update(entity);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<PostResponse>> GetAllByTeamId(string teamId)
        {
            var query = from p in _dbContext.Post
                        join t in _dbContext.Team on p.PostTeamId equals t.TeamId
                        join u in _dbContext.User on p.PostUserId equals u.Id
                        select new { p, u.ImageUrl, u.FullName, p.Comment.Count };
            query = query.Where(x => x.p.PostTeamId == teamId);

            var outPut = await query.Select(x => new PostResponse
            {
                PostId = x.p.PostId,
                PostUserId = x.p.PostUserId,
                PostTeamId = x.p.PostTeamId,
                PostContent = x.p.PostContent,
                PostCreatedAt = x.p.PostCreatedAt,
                PostCommentCount = x.Count,
                PostIsDeleted = x.p.PostIsDeleted,
                PostIsPinned = x.p.PostIsPinned,
                UserAvatar = x.ImageUrl,
                UserName = x.FullName,
            }).ToListAsync();

            return outPut;
        }

        public async Task<List<PostResponse>> GetAllByUserId(string userId)
        {
            var query = from p in _dbContext.Post
                        join u in _dbContext.User on p.PostUserId equals u.Id
                        select new { p, u.ImageUrl, u.Id, u.FullName, p.Comment.Count };

            query = query.Where(x => x.Id == userId);

            var outPut = await query.Select(x => new PostResponse
            {
                PostId = x.p.PostId,
                PostUserId = x.p.PostUserId,
                PostTeamId = x.p.PostTeamId,
                PostContent = x.p.PostContent,
                PostCreatedAt = x.p.PostCreatedAt,
                PostCommentCount = x.Count,
                PostIsDeleted = x.p.PostIsDeleted,
                PostIsPinned = x.p.PostIsPinned,
                UserAvatar = x.ImageUrl,
                UserName = x.FullName,
            }).ToListAsync();

            return outPut;
        }

        public async Task<List<PostResponse>> GetAllByUserTeamId(string userId, string teamId)
        {
            var query = from p in _dbContext.Post
                        where p.PostTeamId == teamId && p.PostUserId == userId
                        select p;

            var outPut = await query.Select(x => new PostResponse
            {
                PostId = x.PostId,
                PostUserId = x.PostUserId,
                PostTeamId = x.PostTeamId,
                PostContent = x.PostContent,
                PostCreatedAt = x.PostCreatedAt,
                PostCommentCount = x.PostCommentCount,
                PostIsDeleted = x.PostIsDeleted,
                PostIsPinned = x.PostIsPinned,
            }).ToListAsync();

            return outPut;
        }

        public async Task<PagedResponse<PostResponse>> GetPaging(RequestParameter parameter)
        {
            var queryUserPost = from p in _dbContext.Post
                                join u in _dbContext.User on p.PostUserId equals u.Id
                                select new { p, u.ImageUrl, u.Id, u.FullName, p.Comment.Count };

            var queryOrder = queryUserPost.OrderByDescending(x => x.p.PostCreatedAt);

            var query = queryOrder.Skip((parameter.PageNumber - 1) * parameter.PageSize)
                .Take(parameter.PageSize);

            var entityList = await query.Select(x => new PostResponse
            {
                PostId = x.p.PostId,
                PostUserId = x.p.PostUserId,
                PostTeamId = x.p.PostTeamId,
                PostContent = x.p.PostContent,
                PostCreatedAt = x.p.PostCreatedAt,
                PostCommentCount = x.Count,
                PostIsDeleted = x.p.PostIsDeleted,
                PostIsPinned = x.p.PostIsPinned,
                UserName = x.FullName,
                UserAvatar = x.ImageUrl,
            }).ToListAsync();

            var outPut = new PagedResponse<PostResponse>(entityList, parameter.PageNumber, parameter.PageSize, await query.CountAsync());

            return outPut;
        }

        public async Task<bool> UpdatePost(string postId, PostRequest postReq)
        {
            var entity = await _dbContext.Post.FindAsync(postId);
            if (entity == null)
                return false;

            entity.PostUserId = postReq.PostUserId;
            entity.PostUserId = postReq.PostUserId;
            entity.PostTeamId = postReq.PostTeamId;
            entity.PostContent = postReq.PostContent;
            entity.PostContent = postReq.PostContent;
            entity.PostCreatedAt = postReq.PostCreatedAt;
            entity.PostCommentCount = postReq.PostCommentCount;
            entity.PostIsDeleted = postReq.PostIsDeleted;
            entity.PostIsPinned = postReq.PostIsPinned;

            _dbContext.Post.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
