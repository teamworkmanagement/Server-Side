﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Post;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/post")]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _repo;
        private readonly ITeamRepository _teamRepo;
        public PostController(IPostRepository repo, ITeamRepository teamRepo)
        {
            _repo = repo;
            _teamRepo = teamRepo;
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var res = await _repo.GetAllByUserId(userId);

            var outPut = new ApiResponse<List<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet("byteamid/{teamId}")]
        public async Task<IActionResult> GetAllByTeamId(string teamId)
        {
            var res = await _repo.GetAllByTeamId(teamId);

            var outPut = new ApiResponse<List<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet("byuserid/{userId}/byteamid/{teamId}")]
        public async Task<IActionResult> GetAllByUserTeamId(string userId, string teamId)
        {
            var res = await _repo.GetAllByUserTeamId(userId, teamId);

            var outPut = new ApiResponse<List<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] RequestParameter parameter)
        {
            var res = await _repo.GetPaging(parameter);

            var outPut = new ApiResponse<PagedResponse<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpPost]
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
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(string postId, [FromForm] PostRequest postReq)
        {
            var res = await _repo.UpdatePost(postId, postReq);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
                Message = !res ? "Sửa thất bại" : "Sửa thành công",
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

        [HttpGet("allforuser")]
        public async Task<IActionResult> GetAllPostForUser(string userId)
        {
            var outPut = new List<PostResponse>();
            var userTeams = await _teamRepo.GetByUserId(userId);
            foreach (var e in userTeams)
            {
                var posts = await _repo.GetAllByTeamId(e.TeamId);
                outPut.AddRange(posts);
            }

            return Ok(new ApiResponse<List<PostResponse>>
            {
                Succeeded = true,
                Data = outPut,
            });
        }

        [HttpGet("paging-multi-user")]
        public async Task<IActionResult> GetMultiPagingUser([FromQuery] PostRequestParameter parameter)
        {
            var res = await _repo.GetPostPagingUser(parameter);

            var outPut = new ApiResponse<PagedResponse<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpGet("paging-multi-team")]
        public async Task<IActionResult> GetPostPagingTeam([FromQuery] PostRequestParameter parameter)
        {
            var res = await _repo.GetPostPagingTeam(parameter);

            var outPut = new ApiResponse<PagedResponse<PostResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpPost("add-react")]
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

        [HttpDelete("delete-react")]
        public async Task<IActionResult> DeleteReact([FromQuery] ReactModel model)
        {
            var res = await _repo.DeleteReact(model);
            return Ok(new ApiResponse<bool>
            {
                Succeeded = res,
                Data = res,
            });
        }

        [HttpGet("search-user")]
        public async Task<IActionResult> SearchUser(string userId, string keyWord)
        {
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = await _repo.SearchUser(userId, keyWord),
            });
        }
    }
}
