using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Comment;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/comment")]
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
            var outPut = new ApiResponse<List<CommentResponse>>()
            {
                Data = await _repo.GetAllByUserId(userId),
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet("byteamid/{teamId}")]
        public async Task<IActionResult> GetAllByTeamId(string teamId)
        {
            var outPut = new ApiResponse<List<CommentResponse>>
            {
                Data = await _repo.GetAllByTeamId(teamId),
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet("byuserid/{userId}/byteamid/{teamId}")]
        public async Task<IActionResult> GetAllByUserTeamId(string userId, string teamId)
        {
            var outPut = new ApiResponse<List<CommentResponse>>
            {
                Data = await _repo.GetAllByUserTeamId(userId, teamId),
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] RequestParameter parameter)
        {
            var outPut = new ApiResponse<PagedResponse<CommentResponse>>
            {
                Data = await _repo.GetPaging(parameter),
                Succeeded = true,
            };
            return Ok(outPut);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromForm] CommentRequest cmtReq)
        {
            var outPut = new ApiResponse<string>
            {
                Data = await _repo.AddComment(cmtReq),
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(string cmtId, [FromForm] CommentRequest cmtReq)
        {
            var outPut = new ApiResponse<bool>
            {
                Data = await _repo.UpdateComment(cmtId, cmtReq),
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            var outPut = new ApiResponse<bool>
            {
                Data = await _repo.DeleteComment(commentId),
                Succeeded = true,
            };

            return Ok(outPut);
        }
    }
}
