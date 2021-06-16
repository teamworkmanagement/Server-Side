using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanList;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using TeamApp.Infrastructure.Persistence.Hubs.Kanban;
using System.Collections.ObjectModel;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class KanbanListRepository : IKanbanListRepository
    {
        private readonly TeamAppContext _dbContext;
        private readonly IHubContext<HubKanbanClient, IHubKanbanClient> _hubKanban;
        public KanbanListRepository(TeamAppContext dbContext, IHubContext<HubKanbanClient, IHubKanbanClient> hubKanban)
        {
            _dbContext = dbContext;
            _hubKanban = hubKanban;
        }
        public async Task<KanbanListUIResponse> AddKanbanList(KanbanListRequest kanbanListRequest)
        {
            var entity = new KanbanList
            {
                KanbanListId = string.IsNullOrEmpty(kanbanListRequest.KanbanListId) ? Guid.NewGuid().ToString() : kanbanListRequest.KanbanListId,
                KanbanListTitle = kanbanListRequest.KanbanListTitle,
                KanbanListBoardBelongedId = kanbanListRequest.KanbanListBoardBelongedId,
                KanbanListRankInBoard = kanbanListRequest.KanbanListRankInBoard,
            };

            await _dbContext.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            var board = await _dbContext.KanbanBoard.FindAsync(kanbanListRequest.KanbanListBoardBelongedId);

            var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                 join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                 where u.Type == "kanban" && (p.ParticipationTeamId == board.KanbanBoardTeamId || p.ParticipationUserId == board.KanbanBoardUserId)
                                 select u.ConnectionId).ToListAsync();

            var response = new KanbanListUIResponse
            {
                KanbanListId = entity.KanbanListId,
                KanbanListTitle = entity.KanbanListTitle,
                KanbanListBoardBelongedId = entity.KanbanListBoardBelongedId,
                KanbanListRankInBoard = entity.KanbanListRankInBoard,
                TaskUIKanbans = new List<Application.DTOs.Task.TaskUIKanban>(),
            };

            var cPush = new ReadOnlyCollection<string>(clients);
            await _hubKanban.Clients.Clients(cPush).AddNewList(response);

            if (check)
            {
                return response;
            }

            return null;
        }

        public async Task<bool> ChangeName(KanbanListChangeNameModel kanbanListChangeNameModel)
        {
            var kbListEntity = await (from kl in _dbContext.KanbanList.AsNoTracking()
                                      where kl.KanbanListId == kanbanListChangeNameModel.KanbanListId
                                      select kl).FirstOrDefaultAsync();

            if (kbListEntity == null)
                return false;

            kbListEntity.KanbanListTitle = kanbanListChangeNameModel.KanbanListName;
            await _dbContext.SingleUpdateAsync(kbListEntity);

            await _dbContext.KanbanList.SingleUpdateAsync(kbListEntity);

            var board = await _dbContext.KanbanBoard.FindAsync(kbListEntity.KanbanListBoardBelongedId);

            var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                 join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                 where u.Type == "kanban" && p.ParticipationIsDeleted == false && (p.ParticipationTeamId == board.KanbanBoardTeamId || p.ParticipationUserId == board.KanbanBoardUserId)
                                 select u.ConnectionId).ToListAsync();

            await _hubKanban.Clients.Clients(clients).RenameList(kanbanListChangeNameModel);

            return true;
        }

        public async Task<bool> RemoveList(KanbanListRequest kanbanListRequest)
        {
            var kbListEntity = await (from kl in _dbContext.KanbanList.AsNoTracking()
                                      where kl.KanbanListId == kanbanListRequest.KanbanListId
                                      select kl).FirstOrDefaultAsync();

            if (kbListEntity == null)
                return false;

            kbListEntity.KanbanListIsDeleted = true;
            await _dbContext.KanbanList.SingleUpdateAsync(kbListEntity);

            var board = await _dbContext.KanbanBoard.FindAsync(kanbanListRequest.KanbanListBoardBelongedId);

            var clients = await (from p in _dbContext.Participation.AsNoTracking()
                                 join u in _dbContext.UserConnection.AsNoTracking() on p.ParticipationUserId equals u.UserId
                                 where u.Type == "kanban" && p.ParticipationIsDeleted == false && (p.ParticipationTeamId == board.KanbanBoardTeamId || p.ParticipationUserId == board.KanbanBoardUserId)
                                 select u.ConnectionId).ToListAsync();

            var response = new KanbanListUIResponse
            {
                KanbanListId = kanbanListRequest.KanbanListId,
                KanbanListBoardBelongedId = kanbanListRequest.KanbanListBoardBelongedId,
            };

            await _hubKanban.Clients.Clients(clients).RemoveList(response);

            return true;
        }
    }
}
