using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanList;


namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IKanbanListRepository
    {
        /// <summary>
        /// Add new KanbanList
        /// </summary>
        /// <param name="kanbanListRequest"></param>
        /// <returns></returns>
        Task<KanbanListResponse> AddKanbanList(KanbanListRequest kanbanListRequest);
    }
}
