using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return Ok(await _repo.GetByUserId(userId));
        }

        [HttpGet("{teamId}")]
        public async Task<IActionResult> GetById(string teamId)
        {
            return Ok(await _repo.GetById(teamId));
        }

        [HttpPost]
        public async Task<IActionResult> AddTeam([FromForm] TeamRequest teamReq)
        {
            return Ok(await _repo.AddTeam(teamReq));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTeam([FromForm] string teamId, [FromForm] TeamRequest teamReq)
        {
            return Ok(await _repo.UpdateTeam(teamId, teamReq));
        }

        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteById(string teamId)
        {
            return Ok(await _repo.DeleteTeam(teamId));
        }


    }
}
