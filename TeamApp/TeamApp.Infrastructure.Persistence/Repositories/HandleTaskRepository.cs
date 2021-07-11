using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamApp.Application.DTOs.HandleTask;
using TeamApp.Application.Utils;
using TeamApp.Application.DTOs.Task;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Kanban;
using TeamApp.Infrastructure.Persistence.Hubs.Notification;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class HandleTaskRepository : IHandleTaskRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubKanbanClient, IHubKanbanClient> _kanbanHub;
        private readonly INotificationRepository _notiRepo;

        public HandleTaskRepository(TeamAppContext dbContext, IHubContext<HubKanbanClient, IHubKanbanClient> kanbanHub, INotificationRepository notiRepo)
        {
            _dbContext = dbContext;
            _kanbanHub = kanbanHub;
            _notiRepo = notiRepo;
        }

        public async Task<bool> ReAssignTask(ReAssignModel reAssignModel)
        {
            var entity = await (from h in _dbContext.HandleTask
                                where h.HandleTaskTaskId == reAssignModel.TaskId
                                select h).FirstOrDefaultAsync();
            var checkExists = false;
            if (entity == null)
            {
                await _dbContext.HandleTask.AddAsync(new HandleTask
                {
                    HandleTaskId = Guid.NewGuid().ToString(),
                    HandleTaskUserId = reAssignModel.CurrentUserId,
                    HandleTaskTaskId = reAssignModel.TaskId,
                    HandleTaskCreatedAt = DateTime.UtcNow,
                    HandleTaskIsDeleted = false,
                });
            }
            else
            {
                if (entity.HandleTaskUserId == reAssignModel.CurrentUserId)
                    checkExists = true;

                entity.HandleTaskUserId = reAssignModel.CurrentUserId;
                _dbContext.HandleTask.Update(entity);
            }


            if (!checkExists)
            {
                var task = await _dbContext.Task.FindAsync(reAssignModel.TaskId);

                var board = await (from b in _dbContext.KanbanBoard.AsNoTracking()
                                   join kl in _dbContext.KanbanList.AsNoTracking() on b.KanbanBoardId equals kl.KanbanListBoardBelongedId
                                   where kl.KanbanListId == task.TaskBelongedId
                                   select b).FirstOrDefaultAsync();

                var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                     join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                     where u.Type == "kanban" && p.ParticipationTeamId == board.KanbanBoardTeamId
                                     select u.ConnectionId).ToListAsync();

                if (reAssignModel.CurrentUserId != null)
                {
                    var user = await _dbContext.User.FindAsync(reAssignModel.CurrentUserId);
                    var obj = new
                    {
                        UserId = user.Id,
                        UserAvatar = string.IsNullOrEmpty(user.ImageUrl) ? $"https://ui-avatars.com/api/?name={user.FullName}" : user.ImageUrl,
                        TaskId = reAssignModel.TaskId,
                        KanbanListId = task.TaskBelongedId,
                        UserFullName = user.FullName,
                    };

                    await _kanbanHub.Clients.Clients(clients).ReAssignUser(obj);

                    //push noti
                    await _notiRepo.PushNotiAssignTask(new AssignNotiModel
                    {
                        ActionUserId = reAssignModel.UserActionId,
                        UserId = reAssignModel.CurrentUserId,
                        TaskId = reAssignModel.TaskId,
                    });
                }
                else
                {
                    var obj = new
                    {
                        UserId = string.Empty,
                        UserAvatar = string.Empty,
                        TaskId = reAssignModel.TaskId,
                        KanbanListId = task.TaskBelongedId
                    };

                    await _kanbanHub.Clients.Clients(clients).ReAssignUser(obj);
                }
            }
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
