using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/post")]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _repo;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public PostController(IPostRepository repo, IAuthenticatedUserService authenticatedUserService)
        {
            _repo = repo;
            _authenticatedUserService = authenticatedUserService;
        }

        /*[HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] RequestParameter parameter)
        {
            var res = await _repo.GetPaging(parameter);

            var outPut = new ApiResponse<PagedResponse<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }*/

        /// <summary>
        /// Create post API
        /// </summary>
        /// <param name="postReq"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(ApiResponse<PostResponse>))]
        public async Task<IActionResult> AddPost(PostRequest postReq)
        {
            var res = await _repo.AddPost(postReq);

            var outPut = new ApiResponse<PostResponse>
            {
                Data = res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Thêm thất bại" : null,
            };

            return Ok(outPut);
        }


        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var res = await _repo.DeletePost(postId);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
                Message = !res ? "Xóa thất bại" : null,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Get post pagination for user API
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("paging-multi-user")]
        [ProducesDefaultResponseType(typeof(ApiResponse<PagedResponse<PostResponse>>))]
        public async Task<IActionResult> GetMultiPagingUser([FromQuery] PostRequestParameter parameter)
        {
            parameter.UserId = _authenticatedUserService.UserId;
            var res = await _repo.GetPostPagingUser(parameter);

            var outPut = new ApiResponse<PagedResponse<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Get post pagination for team API
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("paging-multi-team")]
        [ProducesDefaultResponseType(typeof(ApiResponse<PagedResponse<PostResponse>>))]
        public async Task<IActionResult> GetPostPagingTeam([FromQuery] PostRequestParameter parameter)
        {
            parameter.UserId = _authenticatedUserService.UserId;
            var res = await _repo.GetPostPagingTeam(parameter);

            var outPut = new ApiResponse<PagedResponse<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Add post-react API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("add-react")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> AddReact(ReactModel model)
        {
            var res = await _repo.AddReact(model);
            var outPut = new ApiResponse<string>
            {
                Succeeded = res == null ? false : true,
                Data = res,
                Message = res == null ? "Error while add" : null,
            };
            return Ok(outPut);
        }

        /// <summary>
        /// Delete post-react API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("delete-react")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteReact([FromQuery] ReactModel model)
        {
            var res = await _repo.DeleteReact(model);
            return Ok(new ApiResponse<bool>
            {
                Succeeded = res,
                Data = res,
            });
        }

        /// <summary>
        /// Search user for filter post API
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpGet("search-user")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<UserResponse>>))]
        public async Task<IActionResult> SearchUser(string userId, string keyWord)
        {
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = await _repo.SearchUser(userId, keyWord),
            });
        }
    }
}
