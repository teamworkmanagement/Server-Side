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
using TeamApp.Infrastructure.Persistence.Hubs.Kanban;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<HubPostClient, IHubPostClient> _postHub;
        private readonly IHubContext<HubKanbanClient, IHubKanbanClient> _kanbanHub;

        public CommentRepository(TeamAppContext dbContext, INotificationRepository notificationRepository, IHubContext<HubPostClient, IHubPostClient> postHub, IHubContext<HubKanbanClient, IHubKanbanClient> kanbanHub)
        {
            _dbContext = dbContext;
            _notificationRepository = notificationRepository;
            _kanbanHub = kanbanHub;
            _postHub = postHub;
        }

        public object GetTaskUIKanban(Entities.Task entity) => new
        {
            entity.TaskRankInList,
            KanbanListId = entity.TaskBelongedId,
            entity.TaskId,

            entity.TaskName,
            TaskStartDate = entity.TaskStartDate.FormatTime(),
            TaskDeadline = entity.TaskDeadline.FormatTime(),
            entity.TaskStatus,
            entity.TaskDescription,

            entity.TaskImageUrl,

            CommentsCount = _dbContext.Comment.AsNoTracking().Where(c => c.CommentTaskId == entity.TaskId).Count(),
            FilesCount = _dbContext.File.AsNoTracking().Where(f => f.FileTaskOwnerId == entity.TaskId).Count(),

            entity.TaskCompletedPercent,

            entity.TaskThemeColor,
        };

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

            if (cmtReq.CommentUserTagIds != null && cmtReq.CommentUserTagIds.Count != 0)
                await _notificationRepository.PushNotiCommentTag(new CommentMentionRequest
                {
                    ActionUserId = cmtReq.CommentUserId,
                    UserIds = cmtReq.CommentUserTagIds,
                    PostId = cmtReq.CommentPostId,
                    TaskId = cmtReq.CommentTaskId,
                });

            List<string> clients = new List<string>();
            object taskUIKanban = new { };

            if (!string.IsNullOrEmpty(cmtReq.CommentPostId))
            {
                var query = from p in _dbContext.Participation.AsNoTracking()
                            join uc in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals uc.UserId
                            where p.ParticipationTeamId == cmtReq.CommentTeamId && uc.Type == "post"
                            select uc.ConnectionId;

                query = query.Distinct();
                clients = await query.ToListAsync();
            }
            else
            {
                var task = await _dbContext.Task.FindAsync(cmtReq.CommentTaskId);
                var kblist = await _dbContext.KanbanList.FindAsync(task.TaskBelongedId);
                var board = await _dbContext.KanbanBoard.FindAsync(kblist.KanbanListBoardBelongedId);

                if (!string.IsNullOrEmpty(board.KanbanBoardTeamId))
                {
                    var query = from p in _dbContext.Participation.AsNoTracking()
                                join uc in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals uc.UserId
                                where p.ParticipationTeamId == board.KanbanBoardTeamId && uc.Type == "kanban"
                                && p.ParticipationIsDeleted == false
                                select uc.ConnectionId;

                    query = query.Distinct();
                    clients = await query.ToListAsync();
                }
                else
                {
                    var query =
                                from uc in _dbContext.UserConnection
                                where uc.UserId == board.KanbanBoardUserId && uc.Type == "kanban"
                                select uc.ConnectionId;

                    query = query.Distinct();
                    clients = await query.ToListAsync();
                }

                taskUIKanban = GetTaskUIKanban(task);

            }


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

            if (clients.Count > 0 && cmtReq.CommentTaskId != null)
            {
                await _kanbanHub.Clients.Clients(clients).UpdateTask(taskUIKanban);
            }

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

        public async Task AddMentions(CommentMentionRequest mentionRequest)
        {
            await _notificationRepository.PushNotiCommentTag(mentionRequest);
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
                        orderby c.CommentCreatedAt descending
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
