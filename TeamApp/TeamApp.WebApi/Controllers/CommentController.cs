using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.Comment;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _repo;
        public CommentController(ICommentRepository repo)
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
        public async Task<IActionResult> AddComment([FromForm] CommentRequest cmtReq)
        {
            return Ok(await _repo.AddComment(cmtReq));
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(string cmtId, [FromForm] CommentRequest cmtReq)
        {
            return Ok(await _repo.UpdateComment(cmtId, cmtReq));
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            return Ok(await _repo.DeleteComment(commentId));
        }
    }
}
