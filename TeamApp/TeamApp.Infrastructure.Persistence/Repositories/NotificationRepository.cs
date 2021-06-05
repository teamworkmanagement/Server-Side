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
using TeamApp.Application.DTOs.Notification;
using TeamApp.Application.Utils;
using TeamApp.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Notification;
using System.Collections.ObjectModel;
using Task = System.Threading.Tasks.Task;
using TeamApp.Application.DTOs.Comment;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.DTOs.Task;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly TeamAppContext _dbContext;

        private readonly IHubContext<HubNotificationClient, IHubNotificationClient> _notiHub;
        public NotificationRepository(TeamAppContext dbContext, IHubContext<HubNotificationClient, IHubNotificationClient> notiHub)
        {
            _dbContext = dbContext;
            _notiHub = notiHub;
        }

        public async Task<PagedResponse<NotificationResponse>> GetPaging(NotificationRequestParameter parameter)
        {
            var query = from n in _dbContext.Notification.AsNoTracking()
                        join u in _dbContext.User.AsNoTracking() on n.NotificationUserId equals u.Id
                        orderby n.NotificationCreatedAt descending
                        where n.NotificationUserId == parameter.UserId
                        select n;
            var totalRecords = await query.CountAsync();
            query = query.AsNoTracking().Skip(parameter.SkipItems).Take(parameter.PageSize);

            var entityList = await query.Select(x => new NotificationResponse
            {
                NotificationId = x.NotificationId,
                NotificationUserId = x.NotificationUserId,
                NotificationContent = x.NotificationContent,
                NotificationCreatedAt = x.NotificationCreatedAt.FormatTime(),
                NotificationLink = x.NotificationLink,
                NotificationStatus = x.NotificationStatus,
                NotificationIsDeleted = x.NotificationIsDeleted,
                NotificationGroup = x.NotificationGroup,
                NotificationImage = "https://firebasestorage.googleapis.com/v0/b/fir-fcm-5eb6f.appspot.com/o/notification_500px.png?alt=media&token=e68bc511-fdd4-4f76-90d9-11e86a143f21"
            }).ToListAsync();

            var outPut = new PagedResponse<NotificationResponse>(entityList, parameter.PageSize, totalRecords, skipRows: parameter.SkipItems);

            return outPut;
        }

        public async Task PushNotiAddPostTag(AddPostMentionRequest mentionRequest)
        {
            mentionRequest.UserIds = mentionRequest.UserIds.Distinct().ToList();
            var notiGroup = Guid.NewGuid().ToString();
            List<Notification> notifications = new List<Notification>();

            var clientLists = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                     where uc.Type == "notification" && mentionRequest.UserIds.Contains(uc.UserId)
                                     select uc.ConnectionId).AsNoTracking().ToListAsync();

            Console.WriteLine("count = " + clientLists.Count());
            var readOnlyList = new ReadOnlyCollection<string>(clientLists);

            var link = string.Empty;

            var post = await _dbContext.Post.FindAsync(mentionRequest.PostId);
            link = $"/newsfeed?p={post.PostId}";

            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new
            {
                NotificationGroup = notiGroup,
                NotificationContent = "Bạn vừa được nhắc đến trong 1 bài viết",
                NotificationStatus = false,
                NotificationLink = link,
            });


            foreach (var u in mentionRequest.UserIds)
            {
                notifications.Add(new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    NotificationUserId = u,
                    NotificationGroup = notiGroup,
                    NotificationContent = "Bạn vừa được nhắc đến trong 1 bài viết",
                    NotificationCreatedAt = DateTime.UtcNow,
                    NotificationStatus = false,
                    NotificationIsDeleted = false,
                    NotificationLink = link,
                });
            }

            await _dbContext.BulkInsertAsync(notifications);
        }

        public async Task PushNotiAssignTask(AssignNotiModel assignNotiModel)
        {
            var notiGroup = Guid.NewGuid().ToString();

            var clientLists = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                     where uc.Type == "notification" && uc.UserId == assignNotiModel.UserId
                                     select uc.ConnectionId).AsNoTracking().ToListAsync();

            Console.WriteLine("count = " + clientLists.Count());
            var readOnlyList = new ReadOnlyCollection<string>(clientLists);

            var task = await _dbContext.Task.FindAsync(assignNotiModel.TaskId);
            var kl = await _dbContext.KanbanList.FindAsync(task.TaskBelongedId);
            var link = $"/managetask/teamtasks?b={kl.KanbanListBoardBelongedId}&t={task.TaskId}";

            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new
            {
                NotificationGroup = notiGroup,
                NotificationContent = "Bạn vừa được giao một công việc",
                NotificationStatus = false,
                NotificationLink = link,
            });


            var noti = new Notification
            {
                NotificationId = Guid.NewGuid().ToString(),
                NotificationUserId = assignNotiModel.UserId,
                NotificationGroup = notiGroup,
                NotificationContent = "Bạn vừa được giao một công việc",
                NotificationCreatedAt = DateTime.UtcNow,
                NotificationStatus = false,
                NotificationIsDeleted = false,
                NotificationLink = link,
            };

            await _dbContext.SingleInsertAsync(noti);
        }

        public async Task PushNotiCommentTag(CommentMentionRequest mentionRequest)
        {
            mentionRequest.UserIds = mentionRequest.UserIds.Distinct().ToList();
            var notiGroup = Guid.NewGuid().ToString();
            List<Notification> notifications = new List<Notification>();

            var clientLists = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                     where uc.Type == "notification" && mentionRequest.UserIds.Contains(uc.UserId)
                                     select uc.ConnectionId).AsNoTracking().ToListAsync();

            Console.WriteLine("count = " + clientLists.Count());
            var readOnlyList = new ReadOnlyCollection<string>(clientLists);


            var link = string.Empty;
            if (!string.IsNullOrEmpty(mentionRequest.PostId))
            {
                var post = await _dbContext.Post.FindAsync(mentionRequest.PostId);
                link = $"/newsfeed?p={post.PostId}";
            }
            else
            {
                var task = await _dbContext.Task.FindAsync(mentionRequest.TaskId);
                var kl = await _dbContext.KanbanList.FindAsync(task.TaskBelongedId);
                link = $"/managetask/teamtasks?b={kl.KanbanListBoardBelongedId}&t={task.TaskId}";
            }

            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new
            {
                NotificationGroup = notiGroup,
                NotificationContent = "Bạn vừa được nhắc đến trong 1 bình luận",
                NotificationStatus = false,
                NotificationLink = link,
            });



            foreach (var u in mentionRequest.UserIds)
            {
                notifications.Add(new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    NotificationUserId = u,
                    NotificationGroup = notiGroup,
                    NotificationContent = "Bạn vừa được nhắc đến trong 1 bình luận",
                    NotificationCreatedAt = DateTime.UtcNow,
                    NotificationStatus = false,
                    NotificationIsDeleted = false,
                    NotificationLink = link,
                });
            }

            await _dbContext.BulkInsertAsync(notifications);
        }

        public async Task PushNotiJoinTeam(JoinTeamNotification joinTeamNotification)
        {
            var notiGroup = Guid.NewGuid().ToString();

            var clientLists = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                     where uc.Type == "notification" && uc.UserId == joinTeamNotification.UserId
                                     select uc.ConnectionId).AsNoTracking().ToListAsync();

            Console.WriteLine("count = " + clientLists.Count());
            var readOnlyList = new ReadOnlyCollection<string>(clientLists);

            var link = $"/team/{joinTeamNotification.TeamId}";

            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new
            {
                NotificationGroup = notiGroup,
                NotificationContent = "Bạn vừa được thêm vào một nhóm",
                NotificationStatus = false,
                NotificationLink = link,
            });


            var noti = new Notification
            {
                NotificationId = Guid.NewGuid().ToString(),
                NotificationUserId = joinTeamNotification.UserId,
                NotificationGroup = notiGroup,
                NotificationContent = "Bạn vừa được thêm vào một nhóm",
                NotificationCreatedAt = DateTime.UtcNow,
                NotificationStatus = false,
                NotificationIsDeleted = false,
                NotificationLink = link,
            };

            await _dbContext.SingleInsertAsync(noti);
        }

        public async Task<bool> ReadNotificationSet(ReadNotiModel readNotiModel)
        {
            var entity = await (from n in _dbContext.Notification
                                where n.NotificationGroup == readNotiModel.GroupId && n.NotificationUserId == readNotiModel.UserId
                                select n).FirstOrDefaultAsync();

            if (entity == null)
                return false;

            entity.NotificationStatus = true;
            _dbContext.Notification.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
