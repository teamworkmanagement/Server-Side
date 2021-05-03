using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _repo;
        public CommentController(ICommentRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get comments by userId API
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /*[HttpGet("byuserid/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<CommentResponse>>),200)]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var outPut = new ApiResponse<List<CommentResponse>>()
            {
                Data = await _repo.GetAllByUserId(userId),
                Succeeded = true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Get comment by teamId API
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("byteamid/{teamId}")]
        [ProducesResponseType(typeof(ApiResponse<List<CommentResponse>>), 200)]
        public async Task<IActionResult> GetAllByTeamId(string teamId)
        {
            var outPut = new ApiResponse<List<CommentResponse>>
            {
                Data = await _repo.GetAllByTeamId(teamId),
                Succeeded = true,
            };

            return Ok(outPut);
        }*/

        /// <summary>
        /// Get pagination comment API for post
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<CommentResponse>>), 200)]
        public async Task<IActionResult> GetPaging([FromQuery] CommentRequestParameter parameter)
        {
            var outPut = new ApiResponse<PagedResponse<CommentResponse>>
            {
                Data = await _repo.GetPaging(parameter),
                Succeeded = true,
            };
            return Ok(outPut);
        }

        /// <summary>
        /// Add comment API
        /// </summary>
        /// <param name="cmtReq"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CommentResponse>), 200)]
        public async Task<IActionResult> AddComment(CommentRequest cmtReq)
        {
            var outPut = new ApiResponse<CommentResponse>
            {
                Data = await _repo.AddComment(cmtReq),
                Succeeded = true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Update comment API
        /// </summary>
        /// <param name="cmtId"></param>
        /// <param name="cmtReq"></param>
        /// <returns></returns>
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(string cmtId, CommentRequest cmtReq)
        {
            var outPut = new ApiResponse<bool>
            {
                Data = await _repo.UpdateComment(cmtId, cmtReq),
                Succeeded = true,
            };

            return Ok(outPut);
        }


        /// <summary>
        /// Delete comment API
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
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

        /*[HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentByPostId(string postId)
        {
            var outPut = await _repo.GetAllByPostId(postId);
            return Ok(new ApiResponse<List<CommentResponse>>
            {
                Succeeded = true,
                Data = outPut,
            });
        }*/

        /// <summary>
        /// Get pagination comment for task API
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="skipItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("bytask")]
        [ProducesResponseType(typeof(ApiResponse<List<CommentResponse>>), 200)]
        public async Task<IActionResult> GetCommentByTaskId(string taskId, int skipItems, int pageSize = 3)
        {
            var outPut = await _repo.GetListByTask(taskId, skipItems, pageSize);
            return Ok(new ApiResponse<List<CommentResponse>>
            {
                Data = outPut
            });
        }
    }
}
