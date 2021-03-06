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

            var notis = await query.ToListAsync();
            var entityList = new List<NotificationResponse>();

            foreach (var noti in notis)
            {
                var user = await _dbContext.User.FindAsync(noti.NotificationActionUserId);
                var notiResObj = new NotificationResponse
                {
                    NotificationId = noti.NotificationId,
                    NotificationUserId = noti.NotificationUserId,
                    NotificationContent = noti.NotificationContent,
                    NotificationCreatedAt = noti.NotificationCreatedAt.FormatTime(),
                    NotificationLink = noti.NotificationLink,
                    NotificationStatus = noti.NotificationStatus,
                    NotificationIsDeleted = noti.NotificationIsDeleted,
                    NotificationGroup = noti.NotificationGroup,
                    NotificationImage = "https://firebasestorage.googleapis.com/v0/b/fir-fcm-5eb6f.appspot.com/o/notification_500px.png?alt=media&token=e68bc511-fdd4-4f76-90d9-11e86a143f21",
                    NotificationActionFullName = user == null ? null : user.FullName,
                    NotificationActionAvatar = user == null ? "https://firebasestorage.googleapis.com/v0/b/teamappstorage.appspot.com/o/notification_500px.png?alt=media&token=ef0ec680-83ca-476a-8991-3214e1ec1267" : string.IsNullOrEmpty(user.ImageUrl) ? $"https://ui-avatars.com/api/?name={user.FullName}" : user.ImageUrl,
                };

                entityList.Add(notiResObj);
            }

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

            var actionUser = await _dbContext.User.FindAsync(mentionRequest.ActionUserId);

            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new NotificationResponse
            {
                NotificationActionFullName = actionUser.FullName,
                NotificationActionAvatar = string.IsNullOrEmpty(actionUser.ImageUrl) ? $"https://ui-avatars.com/api/?name={actionUser.FullName}" : actionUser.ImageUrl,
                NotificationGroup = notiGroup,
                NotificationContent = "đã nhắc đến bạn trong 1 bài viết",
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
                    NotificationContent = "đã nhắc đến bạn trong 1 bài viết",
                    NotificationCreatedAt = DateTime.UtcNow,
                    NotificationStatus = false,
                    NotificationIsDeleted = false,
                    NotificationLink = link,
                    NotificationActionUserId = mentionRequest.ActionUserId,
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
            var board = await _dbContext.KanbanBoard.FindAsync(kl.KanbanListBoardBelongedId);
            var link = $"/managetask/teamtasks?gr={board.KanbanBoardTeamId}&b={kl.KanbanListBoardBelongedId}&t={task.TaskId}";

            var actionUser = await _dbContext.User.FindAsync(assignNotiModel.ActionUserId);

            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new
            {
                NotificationActionFullName = actionUser.FullName,
                NotificationActionAvatar = string.IsNullOrEmpty(actionUser.ImageUrl) ? $"https://ui-avatars.com/api/?name={actionUser.FullName}" : actionUser.ImageUrl,
                NotificationGroup = notiGroup,
                NotificationContent = "đã giao cho bạn 1 công việc",
                NotificationStatus = false,
                NotificationLink = link,
                NotificationCreatedAt = DateTime.UtcNow,
            });


            var noti = new Notification
            {
                NotificationId = Guid.NewGuid().ToString(),
                NotificationUserId = assignNotiModel.UserId,
                NotificationGroup = notiGroup,
                NotificationContent = "đã giao cho bạn 1 công việc",
                NotificationCreatedAt = DateTime.UtcNow,
                NotificationStatus = false,
                NotificationIsDeleted = false,
                NotificationLink = link,
                NotificationActionUserId = assignNotiModel.ActionUserId,
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
                var kb = await _dbContext.KanbanBoard.FindAsync(kl.KanbanListBoardBelongedId);
                link = $"/managetask/teamtasks?gr={kb.KanbanBoardTeamId}&b={kl.KanbanListBoardBelongedId}&t={task.TaskId}";
            }

            var actionUser = await _dbContext.User.FindAsync(mentionRequest.ActionUserId);

            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new
            {
                NotificationActionFullName = actionUser.FullName,
                NotificationActionAvatar = string.IsNullOrEmpty(actionUser.ImageUrl) ? $"https://ui-avatars.com/api/?name={actionUser.FullName}" : actionUser.ImageUrl,
                NotificationGroup = notiGroup,
                NotificationContent = "đã nhắc đến bạn trong 1 bình luận",
                NotificationStatus = false,
                NotificationLink = link,
                NotificationCreatedAt = DateTime.UtcNow,
            });



            foreach (var u in mentionRequest.UserIds)
            {
                notifications.Add(new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    NotificationUserId = u,
                    NotificationGroup = notiGroup,
                    NotificationContent = "đã nhắc đến bạn trong 1 bình luận",
                    NotificationCreatedAt = DateTime.UtcNow,
                    NotificationStatus = false,
                    NotificationIsDeleted = false,
                    NotificationLink = link,
                    NotificationActionUserId = mentionRequest.ActionUserId,
                });
            }

            await _dbContext.BulkInsertAsync(notifications);
        }

        public async Task PushNotiJoinTeam(JoinTeamNotification joinTeamNotification)
        {
            if (joinTeamNotification.ActionUserId == null)
                return;
            var notiGroup = Guid.NewGuid().ToString();

            var clientLists = await (from uc in _dbContext.UserConnection.AsNoTracking()
                                     where uc.Type == "notification" && uc.UserId == joinTeamNotification.UserId
                                     select uc.ConnectionId).AsNoTracking().ToListAsync();

            Console.WriteLine("count = " + clientLists.Count());
            var readOnlyList = new ReadOnlyCollection<string>(clientLists);

            var link = $"/team/{joinTeamNotification.TeamId}?tab=teaminfo";

            var actionUser = await _dbContext.User.FindAsync(joinTeamNotification.ActionUserId);

            await _notiHub.Clients.Clients(readOnlyList).SendNoti(new
            {
                NotificationActionFullName = actionUser.FullName,
                NotificationActionAvatar = string.IsNullOrEmpty(actionUser.ImageUrl) ? $"https://ui-avatars.com/api/?name={actionUser.FullName}" : actionUser.ImageUrl,
                NotificationGroup = notiGroup,
                NotificationContent = "đã thêm bạn vào một nhóm",
                NotificationStatus = false,
                NotificationLink = link,
            });


            var noti = new Notification
            {
                NotificationId = Guid.NewGuid().ToString(),
                NotificationUserId = joinTeamNotification.UserId,
                NotificationGroup = notiGroup,
                NotificationContent = "đã thêm bạn vào một nhóm",
                NotificationCreatedAt = DateTime.UtcNow,
                NotificationStatus = false,
                NotificationIsDeleted = false,
                NotificationLink = link,
                NotificationActionUserId = joinTeamNotification.ActionUserId
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

        public async Task PushNoti(NotiRequest notiRequest)
        {
            var notis = new List<Notification>();
            var notiGroup = Guid.NewGuid().ToString();
            var users = await _dbContext.User.Select(u => u.Id).ToListAsync();
            foreach (var u in users)
            {
                notis.Add(new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    NotificationGroup = notiGroup,
                    NotificationContent = notiRequest.NotiContent,
                    NotificationCreatedAt = DateTime.UtcNow,
                    NotificationStatus = false,
                    NotificationUserId = u
                });
            }

            await _dbContext.BulkInsertAsync(notis);

            await _notiHub.Clients.All.SendNoti(new
            {
                NotificationActionAvatar = "https://firebasestorage.googleapis.com/v0/b/teamappstorage.appspot.com/o/notification_500px.png?alt=media&token=ef0ec680-83ca-476a-8991-3214e1ec1267",
                NotificationGroup = notiGroup,
                NotificationContent = notiRequest.NotiContent,
                NotificationStatus = false,
                NotificationCreatedAt = DateTime.UtcNow,
            });
        }

        public async Task Reminder()
        {
            Console.WriteLine("abc");
            var appointments = await (from ap in _dbContext.Appointment.AsNoTracking()
                                      join u in _dbContext.User.AsNoTracking()
                                      on ap.UserCreateId equals u.Id
                                      where ap.Date >= DateTime.UtcNow && ap.Date <= DateTime.UtcNow.AddMinutes(1)
                                      select new { ap, u.ImageUrl, u.FullName }).ToListAsync();

            if (appointments.Count > 0)
            {
                foreach (var appoint in appointments)
                {
                    var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                         join uc in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals uc.UserId
                                         where p.ParticipationIsDeleted == false && p.ParticipationTeamId == appoint.ap.TeamId
                                         select uc.ConnectionId).Distinct().ToListAsync();
                    await _notiHub.Clients.Clients(clients).Reminder(new
                    {
                        Id = appoint.ap.Id,
                        Name = appoint.ap.Name,
                        UserCreateName = appoint.FullName,
                        UserCreateAvatar = string.IsNullOrEmpty(appoint.ImageUrl) ? $"https://ui-avatars.com/api/?name={appoint.FullName}" : appoint.ImageUrl,
                        Date = appoint.ap.Date.FormatTime(),
                        Description = appoint.ap.Description,
                        Type = appoint.ap.Type,
                        TeamId = appoint.ap.TeamId,
                    });
                }
            }
        }
    }
}
