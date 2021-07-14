using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/team")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public TeamController(ITeamRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        /// <summary>
        /// Get teams by user API
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("byuserid/{userId}")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<TeamResponse>>))]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var res = await _repo.GetByUserId(userId);

            var outPut = new ApiResponse<List<TeamResponse>>
            {
                Data = res,
                Succeeded = res == null ? false : true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Get team info detail API
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("{teamId}")]
        [ProducesDefaultResponseType(typeof(ApiResponse<TeamResponse>))]
        public async Task<IActionResult> GetById(string teamId)
        {
            var res = await _repo.GetById(teamId, _authenticatedUserService.UserId);

            var outPut = new ApiResponse<TeamResponse>
            {
                Data = res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Không tồn tại" : null,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Create team API
        /// </summary>
        /// <param name="teamReq"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(ApiResponse<TeamResponse>))]
        public async Task<IActionResult> AddTeam(TeamRequest teamReq)
        {
            var res = await _repo.AddTeam(teamReq);

            var outPut = new ApiResponse<TeamResponse>
            {
                Data = res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Thêm lỗi" : null,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Update Team API
        /// </summary>
        /// <param name="teamUpdateRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UpdateTeam(TeamUpdateRequest teamUpdateRequest)
        {
            var res = await _repo.UpdateTeam(teamUpdateRequest);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
                Message = !res ? "Update lỗi" : null,
            };

            return Ok(outPut);
        }

        /*[HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteById(string teamId)
        {
            var res = await _repo.DeleteTeam(teamId);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
                Message = !res ? "Xóa lỗi" : null,
            };

            return Ok(outPut);
        }*/

        /// <summary>
        /// Get users by team API
        /// </summary>
        /// <param name="userParameter"></param>
        /// <returns></returns>
        [HttpGet("getusers-paging")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<UserResponse>>))]
        public async Task<IActionResult> GetUsersByTeamIdPaging([FromQuery] TeamUserParameter userParameter)
        {
            var outPut = await _repo.GetUsersByTeamIdPagingSearch(userParameter);
            return Ok(new ApiResponse<PagedResponse<UserResponse>>
            {
                Succeeded = outPut != null,
                Data = outPut
            });
        }

        /// <summary>
        /// Join team API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("join-team")]
        [ProducesDefaultResponseType(typeof(ApiResponse<TeamResponse>))]
        public async Task<IActionResult> JoinTeam(JoinTeamRequest request)
        {
            var outPut = await _repo.JoinTeam(request);

            return Ok(new ApiResponse<TeamResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        /// <summary>
        /// Get admin team info API
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("get-admin/{teamId}")]
        [ProducesDefaultResponseType(typeof(ApiResponse<UserResponse>))]
        public async Task<IActionResult> GetAdmin(string teamId)
        {
            var outPut = await _repo.GetAdmin(teamId, _authenticatedUserService.UserId);
            return Ok(new ApiResponse<UserResponse>
            {
                Data = outPut,
                Succeeded = outPut != null,
            });
        }

        /// <summary>
        /// Search user for tag in post API
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("users-for-tag/{teamId}")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<UserResponse>>))]
        public async Task<IActionResult> GetUsersForTag(string teamId)
        {
            var outPut = await _repo.GetUsersForTag(teamId);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        /// <summary>
        /// Get boards of team API
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("boards-by-team/{teamId}")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<KanbanBoardResponse>>))]
        public async Task<IActionResult> GetBoardsByTeamId(string teamId)
        {
            var outPut = await _repo.GetBoardsByTeam(teamId);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Succeeded = outPut != null,
                Data = outPut,
            });
        }

        /// <summary>
        /// List recommend team in newsfeed API
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("teams-recommend-user/{userId}")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<TeamRecommendModel>>))]
        public async Task<IActionResult> GetTeamsRecommedUser(string userId)
        {
            var outPut = await _repo.GetRecommendTeamForUser(userId);
            return Ok(new ApiResponse<List<TeamRecommendModel>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        /// <summary>
        /// Change leader of team API
        /// </summary>
        /// <param name="changeTeamAdminModel"></param>
        /// <returns></returns>
        [HttpPost("change-leader")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeTeamLeader([FromBody] ChangeTeamAdminModel changeTeamAdminModel)
        {
            var outPut = await _repo.ChangeTeamLeader(changeTeamAdminModel);
            return Ok(
                new ApiResponse<bool>
                {
                    Succeeded = outPut,
                    Data = outPut,
                });
        }
    }
}
