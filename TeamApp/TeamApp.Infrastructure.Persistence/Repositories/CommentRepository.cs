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
using TeamApp.Application.Utils;
using Task = System.Threading.Tasks.Task;
using TeamApp.Infrastructure.Persistence.Hubs.Post;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<HubPostClient, IHubPostClient> _postHub;

        public CommentRepository(TeamAppContext dbContext, INotificationRepository notificationRepository, IHubContext<HubPostClient, IHubPostClient> postHub)
        {
            _dbContext = dbContext;
            _notificationRepository = notificationRepository;
            _postHub = postHub;
        }

        public async Task<CommentResponse> AddComment(CommentRequest cmtReq)
        {
            var entity = new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                CommentPostId = cmtReq.CommentPostId,
                CommentTaskId = cmtReq.CommentTaskId,
                CommentUserId = cmtReq.CommentUserId,
                CommentContent = cmtReq.CommentContent,
                CommentCreatedAt = cmtReq.CommentCreatedAt,
                CommentIsDeleted = cmtReq.CommentIsDeleted,
            };

            await _dbContext.Comment.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync();

            if (cmtReq.CommentUserTagIds.Count != 0)
                await _notificationRepository.PushNoti(cmtReq.CommentUserTagIds, "Thông báo", "Bạn vừa được nhắc đến trong 1 bình luận");


            var query = from p in _dbContext.Participation.AsNoTracking()
                        join uc in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals uc.UserId
                        where p.ParticipationTeamId == cmtReq.CommentTeamId && uc.Type == "post" && uc.UserId != cmtReq.CommentUserId
                        select uc.ConnectionId;

            query = query.Distinct();
            var clients = await query.ToListAsync();

            var readOnlyStr = new ReadOnlyCollection<string>(clients);

            await _postHub.Clients.Clients(readOnlyStr).NewComment(new CommentResponse
            {
                CommentId = entity.CommentId,
                CommentPostId = entity.CommentPostId,
                CommentUserId = entity.CommentUserId,
                CommentContent = entity.CommentContent,
                CommentTaskId = entity.CommentTaskId,
                CommentCreatedAt = entity.CommentCreatedAt,
                CommentIsDeleted = entity.CommentIsDeleted,
                UserAvatar = cmtReq.CommentUserAvatar,
                UserName = cmtReq.CommentUserName,
            });

            if (check > 0)
            {

                return new CommentResponse
                {
                    CommentId = entity.CommentId,
                    CommentPostId = entity.CommentPostId,
                    CommentUserId = entity.CommentUserId,
                    CommentContent = entity.CommentContent,
                    CommentTaskId = entity.CommentTaskId,
                    CommentCreatedAt = entity.CommentCreatedAt,
                    CommentIsDeleted = entity.CommentIsDeleted,
                    UserAvatar = cmtReq.CommentUserAvatar,
                    UserName = cmtReq.CommentUserName,
                };
            }
            return null;
        }

        public async Task AddMentions(List<string> userIds)
        {
            await _notificationRepository.PushNoti(userIds, "Tag", "Bạn đã được đề cập đến trong một bình luận");
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

        public async Task<List<CommentResponse>> GetAllByPostId(string postId)
        {
            var query = from c in _dbContext.Comment
                        join u in _dbContext.User on c.CommentUserId equals u.Id
                        select new { c, u.ImageUrl, u.FullName };

            var outPut = query.Where(x => x.c.CommentPostId == postId);

            return await outPut.Select(x => new CommentResponse
            {
                CommentId = x.c.CommentId,
                CommentPostId = x.c.CommentPostId,
                CommentUserId = x.c.CommentUserId,
                CommentContent = x.c.CommentContent,
                CommentCreatedAt = x.c.CommentCreatedAt.FormatTime(),
                CommentIsDeleted = x.c.CommentIsDeleted,
                UserAvatar = x.ImageUrl,
                UserName = x.FullName,
            }).ToListAsync();
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
                CommentCreatedAt = x.CommentCreatedAt.FormatTime(),
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
                CommentCreatedAt = x.CommentCreatedAt.FormatTime(),
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
                CommentCreatedAt = x.CommentCreatedAt.FormatTime(),
                CommentIsDeleted = x.CommentIsDeleted,
            }).ToListAsync();

            return outPut;
        }

        public async Task<List<CommentResponse>> GetListByTask(string taskId, int skipItems = 0, int pageSize = 3)
        {
            var query = from c in _dbContext.Comment.AsNoTracking()
                        join t in _dbContext.Task.AsNoTracking() on c.CommentTaskId equals t.TaskId
                        join u in _dbContext.User.AsNoTracking() on c.CommentUserId equals u.Id
                        where t.TaskId == taskId
                        select new { c, u.FullName, u.ImageUrl };

            query = query.AsNoTracking().Skip(skipItems).Take(pageSize);

            var entityList = await query.Select(x => new CommentResponse
            {
                CommentId = x.c.CommentId,
                CommentPostId = x.c.CommentPostId,
                CommentUserId = x.c.CommentUserId,
                CommentContent = x.c.CommentContent,
                CommentTaskId = x.c.CommentTaskId,
                CommentCreatedAt = x.c.CommentCreatedAt.FormatTime(),
                CommentIsDeleted = x.c.CommentIsDeleted,
                UserAvatar = x.ImageUrl,
                UserName = x.FullName,
            }).ToListAsync();

            return entityList;
        }

        public async Task<PagedResponse<CommentResponse>> GetPaging(CommentRequestParameter parameter)
        {
            var query = from c in _dbContext.Comment
                        join u in _dbContext.User on c.CommentUserId equals u.Id
                        select new { c, u.ImageUrl, u.FullName };

            var outQuery = query.Where(x => x.c.CommentPostId == parameter.PostId);

            var queryO = outQuery.OrderByDescending(x => x.c.CommentCreatedAt);
            var queryEnd = queryO.Skip(parameter.SkipItems).Take(parameter.PageSize);

            var entityList = await queryEnd.Select(x => new CommentResponse
            {
                CommentId = x.c.CommentId,
                CommentPostId = x.c.CommentPostId,
                CommentUserId = x.c.CommentUserId,
                CommentContent = x.c.CommentContent,
                CommentCreatedAt = x.c.CommentCreatedAt.FormatTime(),
                CommentIsDeleted = x.c.CommentIsDeleted,
                UserAvatar = string.IsNullOrEmpty(x.ImageUrl) ? $"https://ui-avatars.com/api/?name={x.FullName}" : x.ImageUrl,
                UserName = x.FullName,
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
