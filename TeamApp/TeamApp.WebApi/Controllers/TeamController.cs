using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.KanbanBoard;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.DTOs.User;
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
        public TeamController(ITeamRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("byuserid/{userId}")]
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

        [HttpGet("{teamId}")]
        public async Task<IActionResult> GetById(string teamId)
        {
            var res = await _repo.GetById(teamId);

            var outPut = new ApiResponse<TeamResponse>
            {
                Data = res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Không tồn tại" : null,
            };

            return Ok(outPut);
        }

        [HttpPost]
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

        [HttpPut]
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

        [HttpDelete("{teamId}")]
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
        }

        [HttpGet("getusers-paging")]
        public async Task<IActionResult> GetUsersByTeamIdPaging([FromQuery] TeamUserParameter userParameter)
        {
            var outPut = await _repo.GetUsersByTeamIdPaging(userParameter);
            return Ok(new ApiResponse<PagedResponse<UserResponse>>
            {
                Succeeded = outPut != null,
                Data = outPut
            });
        }

        [HttpPost("join-team")]
        public async Task<IActionResult> JoinTeam(JoinTeamRequest request)
        {
            var outPut = await _repo.JoinTeam(request);

            return Ok(new ApiResponse<TeamResponse>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut,
            });
        }

        [HttpGet("get-admin/{teamId}")]
        public async Task<IActionResult> GetAdmin(string teamId)
        {
            var outPut = await _repo.GetAdmin(teamId);
            return Ok(new ApiResponse<UserResponse>
            {
                Data = outPut,
                Succeeded = outPut != null,
            });
        }

        [HttpGet("users-for-tag/{teamId}")]
        public async Task<IActionResult> GetUsersForTag(string teamId)
        {
            var outPut = await _repo.GetUsersForTag(teamId);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }

        [HttpGet("boards-by-team/{teamId}")]
        public async Task<IActionResult> GetBoardsByTeamId(string teamId)
        {
            var outPut = await _repo.GetBoardsByTeam(teamId);
            return Ok(new ApiResponse<List<KanbanBoardResponse>>
            {
                Succeeded = outPut != null,
                Data = outPut,
            });
        }

        [HttpGet("teams-recommend-user/{userId}")]
        public async Task<IActionResult> GetTeamsRecommedUser(string userId)
        {
            var outPut = await _repo.GetRecommendTeamForUser(userId);
            return Ok(new ApiResponse<List<TeamRecommendModel>>
            {
                Data = outPut,
                Succeeded = true,
            });
        }
    }
}
