using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return Ok(await _repo.GetAllByTeamId(teamId));
        }

        [HttpGet("team/{userId}")]
        public async Task<IActionResult> GetTeamByUserId(string userId)
        {
            return Ok(await _repo.GetTeamByUserId(userId));
        }

        [HttpDelete("{userId}/{teamId}")]
        public async Task<IActionResult> DeleteParticipation(string userId, string teamId)
        {
            return Ok(await _repo.DeleteParticipation(userId, teamId));
        }
    }
}
