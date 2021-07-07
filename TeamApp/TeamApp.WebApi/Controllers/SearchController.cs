using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.AppSearch;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public SearchController(ISearchRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        /// <summary>
        /// Search tasks for user API
        /// </summary>
        /// <param name="appSearchRequest"></param>
        /// <returns></returns>
        [HttpGet("tasks")]
        [ProducesDefaultResponseType(typeof(ApiResponse<PagedResponse<TaskSearchResponse>>))]
        public async Task<IActionResult> SearchTasks([FromQuery] AppSearchRequest appSearchRequest)
        {
            appSearchRequest.UserId = _authenticatedUserService.UserId;
            var outPut = await _repo.SearchTasks(appSearchRequest);
            return Ok(new ApiResponse<PagedResponse<TaskSearchResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        /// <summary>
        /// Search teams for user API
        /// </summary>
        /// <param name="appSearchRequest"></param>
        /// <returns></returns>
        [HttpGet("teams")]
        [ProducesDefaultResponseType(typeof(ApiResponse<PagedResponse<TeamSearchResponse>>))]
        public async Task<IActionResult> SearchTeams([FromQuery] AppSearchRequest appSearchRequest)
        {
            appSearchRequest.UserId = _authenticatedUserService.UserId;
            var outPut = await _repo.SearchTeams(appSearchRequest);
            return Ok(new ApiResponse<PagedResponse<TeamSearchResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        /// <summary>
        /// Search chats for user API
        /// </summary>
        /// <param name="appSearchRequest"></param>
        /// <returns></returns>
        [ProducesDefaultResponseType(typeof(ApiResponse<PagedResponse<GroupChatSearchResponse>>))]
        [HttpGet("chats")]
        public async Task<IActionResult> SearchChats([FromQuery] AppSearchRequest appSearchRequest)
        {
            appSearchRequest.UserId = _authenticatedUserService.UserId;
            var outPut = await _repo.SearchGroupChats(appSearchRequest);
            return Ok(new ApiResponse<PagedResponse<GroupChatSearchResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        /// <summary>
        /// Search boards for user API
        /// </summary>
        /// <param name="appSearchRequest"></param>
        /// <returns></returns>
        [HttpGet("boards")]
        [ProducesDefaultResponseType(typeof(ApiResponse<PagedResponse<BoardSearchResponse>>))]
        public async Task<IActionResult> SearchBoards([FromQuery] AppSearchRequest appSearchRequest)
        {
            appSearchRequest.UserId = _authenticatedUserService.UserId;
            var outPut = await _repo.SearchBoards(appSearchRequest);
            return Ok(new ApiResponse<PagedResponse<BoardSearchResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }
    }
}
