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
using TeamApp.Application.DTOs.Comment;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TeamAppContext _dbContext;

        public CommentRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddComment(CommentRequest cmtReq)
        {
            var entity = new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                CommentPostId = cmtReq.CommentPostId,
                CommentUserId = cmtReq.CommentUserId,
                CommentContent = cmtReq.CommentContent,
                CommentCreatedAt = cmtReq.CommentCreatedAt,
                CommentIsDeleted = cmtReq.CommentIsDeleted,
            };

            await _dbContext.Comment.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.CommentId;
        }

        public async Task<bool> DeleteComment(string cmtId)
        {
            var entity = await _dbContext.Comment.FindAsync(cmtId);
            if (entity == null)
                return false;
            entity.CommentIsDeleted = true;
            _dbContext.Comment.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<CommentResponse>> GetAllByTeamId(string teamId)
        {
            var query = from c in _dbContext.Comment
                        join p in _dbContext.Post on c.CommentPostId equals p.PostId
                        where p.PostTeamId == teamId
                        select c;

            var outPut = await query.Select(x => new CommentResponse
            {
                CommentId = x.CommentId,
                CommentPostId = x.CommentPostId,
                CommentUserId = x.CommentUserId,
                CommentContent = x.CommentContent,
                CommentCreatedAt = x.CommentCreatedAt,
                CommentIsDeleted = x.CommentIsDeleted,
            }).ToListAsync();

            return outPut;
        }

        public async Task<List<CommentResponse>> GetAllByUserId(string userId)
        {
            var query = from c in _dbContext.Comment
                        join p in _dbContext.Post on c.CommentPostId equals p.PostId
                        where p.PostUserId == userId
                        select c;

            var outPut = await query.Select(x => new CommentResponse
            {
                CommentId = x.CommentId,
                CommentPostId = x.CommentPostId,
                CommentUserId = x.CommentUserId,
                CommentContent = x.CommentContent,
                CommentCreatedAt = x.CommentCreatedAt,
                CommentIsDeleted = x.CommentIsDeleted,
            }).ToListAsync();

            return outPut;
        }

        public async Task<List<CommentResponse>> GetAllByUserTeamId(string userId, string teamId)
        {
            var query = from c in _dbContext.Comment
                        join p in _dbContext.Post on c.CommentPostId equals p.PostId
                        where p.PostTeamId == teamId && p.PostUserId == userId
                        select c;

            var outPut = await query.Select(x => new CommentResponse
            {
                CommentId = x.CommentId,
                CommentPostId = x.CommentPostId,
                CommentUserId = x.CommentUserId,
                CommentContent = x.CommentContent,
                CommentCreatedAt = x.CommentCreatedAt,
                CommentIsDeleted = x.CommentIsDeleted,
            }).ToListAsync();

            return outPut;
        }

        public async Task<PagedResponse<CommentResponse>> GetPaging(RequestParameter parameter)
        {
            var query = _dbContext.Comment.Skip(parameter.PageSize * parameter.PageNumber).Take(parameter.PageSize);

            var entityList = await query.Select(x => new CommentResponse
            {
                CommentId = x.CommentId,
                CommentPostId = x.CommentPostId,
                CommentUserId = x.CommentUserId,
                CommentContent = x.CommentContent,
                CommentCreatedAt = x.CommentCreatedAt,
                CommentIsDeleted = x.CommentIsDeleted,
            }).ToListAsync();

            var outPut = new PagedResponse<CommentResponse>(entityList, parameter.PageNumber, parameter.PageSize, await query.CountAsync());

            return outPut;
        }

        public async Task<bool> UpdateComment(string cmtId, CommentRequest cmtReq)
        {
            var entity = await _dbContext.Comment.FindAsync(cmtId);
            if (entity == null)
                return false;

            entity.CommentPostId = cmtReq.CommentPostId;
            entity.CommentUserId = cmtReq.CommentUserId;
            entity.CommentContent = cmtReq.CommentContent;
            entity.CommentCreatedAt = cmtReq.CommentCreatedAt;
            entity.CommentIsDeleted = cmtReq.CommentIsDeleted;

            _dbContext.Comment.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
