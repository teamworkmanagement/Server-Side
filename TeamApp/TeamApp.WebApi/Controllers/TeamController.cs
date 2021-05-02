using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    //[Authorize]
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
        public async Task<IActionResult> AddTeam([FromForm] TeamRequest teamReq)
        {
            var res = await _repo.AddTeam(teamReq);

            var outPut = new ApiResponse<string>
            {
                Data = res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Thêm lỗi" : null,
            };

            return Ok(outPut);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTeam([FromForm] string teamId, [FromForm] TeamRequest teamReq)
        {
            var res = await _repo.UpdateTeam(teamId, teamReq);

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

        [HttpGet("getalluser/{teamId}")]
        public async Task<IActionResult> GetByTeamId(string teamId)
        {
            var outPut = await _repo.GetAllByTeamId(teamId);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Succeeded = outPut == null ? false : true,
                Data = outPut
            });
        }
    }
}
