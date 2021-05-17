using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanList;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using System.Linq;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class KanbanListRepository : IKanbanListRepository
    {
        private readonly TeamAppContext _dbContext;
        public KanbanListRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<KanbanListUIResponse> AddKanbanList(KanbanListRequest kanbanListRequest)
        {
            var entity = new KanbanList
            {
                KanbanListId = string.IsNullOrEmpty(kanbanListRequest.KanbanListId) ? Guid.NewGuid().ToString() : kanbanListRequest.KanbanListId,
                KanbanListTitle = kanbanListRequest.KanbanListTitle,
                KanbanListBoardBelongedId = kanbanListRequest.KanbanListBoardBelongedId,
                KanbanListOrderInBoard = kanbanListRequest.KanbanListOrderInBoard,
            };

            await _dbContext.AddAsync(entity);
            var check = await _dbContext.SaveChangesAsync() > 0;

            if (check)
            {
                return new KanbanListUIResponse
                {
                    KanbanListId = entity.KanbanListId,
                    KanbanListTitle = entity.KanbanListTitle,
                    KanbanListBoardBelongedId = entity.KanbanListBoardBelongedId,
                    KanbanListOrderInBoard = entity.KanbanListOrderInBoard,
                    TaskUIKanbans = new List<Application.DTOs.Task.TaskUIKanban>(),
                };
            }

            return null;
        }

        public async Task<bool> RemoveList(string kbListId)
        {
            var kbListEntity = await (from kl in _dbContext.KanbanList.AsNoTracking()
                                      where kl.KanbanListId == kbListId
                                      select kl).FirstOrDefaultAsync();

            if (kbListEntity == null)
                return false;

            kbListEntity.KanbanListIsDeleted = true;
            await _dbContext.KanbanList.SingleUpdateAsync(kbListEntity);

            var kbListsBoard = await (from kl in _dbContext.KanbanList
                                      join k in _dbContext.KanbanBoard.AsNoTracking() on kl.KanbanListBoardBelongedId equals k.KanbanBoardId
                                      where k.KanbanBoardId == kbListEntity.KanbanListBoardBelongedId
                                      select kl).ToListAsync();

            var newListsBoard = new List<KanbanList>();
            foreach (var e in kbListsBoard)
            {
                if (e.KanbanListOrderInBoard > kbListEntity.KanbanListOrderInBoard)
                {
                    e.KanbanListOrderInBoard--;
                    newListsBoard.Add(e);
                }
            }

            await _dbContext.BulkUpdateAsync(newListsBoard);

            return true;
        }
    }
}
