using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
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

        [HttpGet("search-boards")]
        public async Task<IActionResult> SearchKanbanBoards([FromQuery] SearchBoardModel searchBoardModel)
        {
            var outPut = await _repo.SearchKanbanBoards(searchBoardModel);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }
    }
}
