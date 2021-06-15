using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Paricipation;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/participation")]
    public class ParticipationController : ControllerBase
    {
        private readonly IParticipationRepository _repo;
        public ParticipationController(IParticipationRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("byteamid/{teamId}")]
        public async Task<IActionResult> GetByTeamId(string teamId)
        {
            var res = await _repo.GetAllByTeamId(teamId);

            var outPut = new ApiResponse<List<ParticipationResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet("team/{userId}")]
        public async Task<IActionResult> GetTeamByUserId(string userId)
        {
            var res = await _repo.GetTeamByUserId(userId);

            var outPut = new ApiResponse<List<string>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteParticipation(ParticipationDeleteRequest participationDeleteRequest)
        {
            var res = await _repo.DeleteParticipation(participationDeleteRequest);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
            };

            return Ok(outPut);
        }

        [HttpPost]
        public async Task<IActionResult> AddParticipation(ParticipationRequest participationRequest)
        {
            var outPut = await _repo.AddParticipation(participationRequest);
            return Ok(new ApiResponse<ParticipationResponse>
            {
                Data=outPut,
                Succeeded=outPut==null?false:true,
            });
        }
    }
}
