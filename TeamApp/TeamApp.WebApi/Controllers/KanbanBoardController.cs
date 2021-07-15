using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.Task;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/kanbanboard")]
    public class KanbanBoardController : ControllerBase
    {
        private readonly IKanbanBoardRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public KanbanBoardController(IKanbanBoardRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }


        /// <summary>
        /// Add kanbanboard API
        /// </summary>
        /// <param name="kanbanBoardRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(ApiResponse<KanbanBoardResponse>))]
        public async Task<IActionResult> AddKanbanBoard(KanbanBoardRequest kanbanBoardRequest)
        {
            var outPut = await _repo.AddKanbanBoard(kanbanBoardRequest);
            return Ok(new ApiResponse<KanbanBoardResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        /// <summary>
        /// Get kanbanboard data for UI API
        /// </summary>
        /// <param name="boardUIRequest"></param>
        /// <returns></returns>
        [HttpGet("ui")]
        [ProducesDefaultResponseType(typeof(KanbanBoardUIResponse))]
        public async Task<IActionResult> GetKanbanBoardUI([FromQuery] KanbanBoardUIRequest boardUIRequest)
        {
            var userId = _authenticatedUserService.UserId;
            var outPut = await _repo.GetKanbanBoardUI(userId, boardUIRequest);

            return Ok(new ApiResponse<KanbanBoardUIResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        /// <summary>
        /// Swap kanbanlist API
        /// </summary>
        /// <param name="swapListModel"></param>
        /// <returns></returns>
        [HttpPost("swap-list")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> SwapListKanban(SwapListModel swapListModel)
        {
            var outPut = await _repo.SwapListKanban(swapListModel);
            return Ok(new ApiResponse<bool>
            {
                Succeeded = outPut,
                Data = outPut,
            });
        }

        /// <summary>
        /// Get kanban boards of team user joined API
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("team-boards/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<KanbanBoardResponse>>), 200)]
        public async Task<IActionResult> GetKanbanBoardForUserTeams(string userId)
        {
            var outPut = await _repo.GetBoardForUserTeams(userId);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        /// <summary>
        /// Get kanban boards of user API
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user-boards/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<KanbanBoardResponse>>), 200)]
        public async Task<IActionResult> GetKanbanBoardForUser(string userId)
        {
            var outPut = await _repo.GetBoardForUser(userId);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        /// <summary>
        /// Get kanban boards for team API
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("teamboards/{teamId}")]
        [ProducesResponseType(typeof(ApiResponse<List<KanbanBoardResponse>>), 200)]
        public async Task<IActionResult> GetBoardsForTeam(string teamId)
        {
            var outPut = await _repo.GetBoardsForTeam(teamId);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        /// <summary>
        /// Search kanban boards API
        /// </summary>
        /// <param name="searchBoardModel"></param>
        /// <returns></returns>
        [HttpGet("search-boards")]
        [ProducesResponseType(typeof(ApiResponse<List<KanbanBoardResponse>>), 200)]
        public async Task<IActionResult> SearchKanbanBoards([FromQuery] SearchBoardModel searchBoardModel)
        {
            var outPut = await _repo.SearchKanbanBoards(searchBoardModel);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        /// <summary>
        /// Search list of tags in board API
        /// </summary>
        /// <param name="taskSearchModel"></param>
        /// <returns></returns>
        [HttpGet("search-tasklist-inboards")]
        [ProducesResponseType(typeof(ApiResponse<List<TaskUIKanban>>), 200)]
        public async Task<IActionResult> SearchTaskListInBoard([FromQuery] TaskSearchModel taskSearchModel)
        {
            var outPut = await _repo.SearchTasksListInBoard(taskSearchModel);
            return Ok(new ApiResponse<List<TaskUIKanban>>
            {
                Succeeded = true,
                Data = outPut,
            });
        }

        [HttpPost("rebalance-task")]
        public async Task<IActionResult> RebalanceTask([FromBody] RebalanceTaskModel rebalanceTaskModel)
        {
            var outPut = await _repo.RebalanceTask(rebalanceTaskModel);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }

        [HttpPost("rebalance-list")]
        public async Task<IActionResult> RebalanceList([FromBody] RebalanceListModel rebalanceListModel)
        {
            var outPut = await _repo.RebalanceList(rebalanceListModel);
            return Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            });
        }
    }
}
