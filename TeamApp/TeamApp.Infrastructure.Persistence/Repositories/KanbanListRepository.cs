using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanList;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class KanbanListRepository : IKanbanListRepository
    {
        private readonly TeamAppContext _dbContext;
        public KanbanListRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<KanbanListResponse> AddKanbanList(KanbanListRequest kanbanListRequest)
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
                return new KanbanListResponse
                {
                    KanbanListId = entity.KanbanListId,
                    KanbanListTitle = entity.KanbanListTitle,
                    KanbanListBoardBelongedId = entity.KanbanListBoardBelongedId,
                    KanbanListOrderInBoard = entity.KanbanListOrderInBoard,
                };
            }

            return null;
        }
    }
}
