using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
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
        public KanbanBoardController(IKanbanBoardRepository repo)
        {
            _repo = repo;
        }


        [HttpPost]
        public async Task<IActionResult> AddKanbanBoard(KanbanBoardRequest kanbanBoardRequest)
        {
            var outPut = await _repo.AddKanbanBoard(kanbanBoardRequest);
            return Ok(new ApiResponse<KanbanBoardResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        [HttpGet("ui/{boardId}")]
        [ProducesDefaultResponseType(typeof(KanbanBoardUIResponse))]
        public async Task<IActionResult> GetKanbanBoardUI(string boardId)
        {
            var outPut = await _repo.GetKanbanBoardUI(boardId);

            return Ok(new ApiResponse<KanbanBoardUIResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        [HttpPost("swap-list")]
        public async Task<IActionResult> SwapListKanban(SwapListModel swapListModel)
        {
            var outPut = await _repo.SwapListKanban(swapListModel);
            return Ok(new ApiResponse<bool>
            {
                Succeeded = outPut,
                Data = outPut,
            });
        }

        [HttpGet("team-boards/{userId}")]
        public async Task<IActionResult> GetKanbanBoardForUserTeams(string userId)
        {
            var outPut = await _repo.GetBoardForUserTeams(userId);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        [HttpGet("user-boards/{userId}")]
        public async Task<IActionResult> GetKanbanBoardForUser(string userId)
        {
            var outPut = await _repo.GetBoardForUser(userId);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        [HttpGet("teamboards/{teamId}")]
        public async Task<IActionResult> GetBoardsForTeam(string teamId)
        {
            var outPut = await _repo.GetBoardsForTeam(teamId);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }
    }
}
