using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _repo;
        public PostController(IPostRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            return Ok(await _repo.GetAllByUserId(userId));
        }

        [HttpGet("byteamid/{teamId}")]
        public async Task<IActionResult> GetAllByTeamId(string teamId)
        {
            return Ok(await _repo.GetAllByTeamId(teamId));
        }

        [HttpGet("byuserid/{userId}/byteamid/{teamId}")]
        public async Task<IActionResult> GetAllByUserTeamId(string userId, string teamId)
        {
            return Ok(await _repo.GetAllByUserTeamId(userId, teamId));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] RequestParameter parameter)
        {
            return Ok(await _repo.GetPaging(parameter));
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromForm] PostRequest postReq)
        {
            return Ok(await _repo.AddPost(postReq));
        }
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(string postId, [FromForm] PostRequest postReq)
        {
            return Ok(await _repo.UpdatePost(postId, postReq));
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            return Ok(await _repo.DeletePost(postId));
        }
    }
}
