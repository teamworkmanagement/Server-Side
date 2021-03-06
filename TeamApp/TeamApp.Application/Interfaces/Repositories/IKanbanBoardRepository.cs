using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.KanbanList;
using TeamApp.Application.DTOs.Task;

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
        Task<KanbanBoardUIResponse> GetKanbanBoardUI(string userId, KanbanBoardUIRequest boardId);
        /// <summary>
        /// Swap liskanban
        /// </summary>
        /// <param name="swapListModel"></param>
        /// <returns></returns>
        Task<bool> SwapListKanban(SwapListModel swapListModel);
        Task<List<KanbanBoardResponse>> GetBoardForUserTeams(string userId);
        Task<List<KanbanBoardResponse>> GetBoardForUser(string userId);
        Task<List<KanbanBoardResponse>> GetBoardsForTeam(string teamId);
        Task<List<KanbanBoardResponse>> SearchKanbanBoards(SearchBoardModel searchBoardModel);

        Task<KanbanBoardUIResponse> SearchTasks(TaskSearchModel taskSearchModel);
        Task<List<TaskUIKanban>> SearchTasksListInBoard(TaskSearchModel taskSearchModel);
        Task<bool> RebalanceTask(RebalanceTaskModel rebalanceTaskModel);
        Task<bool> RebalanceList(RebalanceListModel rebalanceListModel);
    }
}
