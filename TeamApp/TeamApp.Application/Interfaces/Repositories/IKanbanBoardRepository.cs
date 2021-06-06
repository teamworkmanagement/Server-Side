using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.KanbanList;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface IKanbanBoardRepository
    {
        /// <summary>
        /// Add new KanbanBoard
        /// </summary>
        /// <param name="kanbanBoardRequest"></param>
        /// <returns></returns>
        Task<KanbanBoardResponse> AddKanbanBoard(KanbanBoardRequest kanbanBoardRequest);
        /// <summary>
        /// Get for show on UI 1st load
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        Task<KanbanBoardUIResponse> GetKanbanBoardUI(KanbanBoardUIRequest boardId);
        /// <summary>
        /// Swap liskanban
        /// </summary>
        /// <param name="swapListModel"></param>
        /// <returns></returns>
        Task<bool> SwapListKanban(SwapListModel swapListModel);
        Task<List<KanbanBoardResponse>> GetBoardForUserTeams(string userId);
        Task<List<KanbanBoardResponse>> GetBoardForUser(string userId);
        Task<List<KanbanBoardResponse>> GetBoardsForTeam(string teamId);
    }
}
