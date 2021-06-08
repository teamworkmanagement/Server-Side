using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.User;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        public UserController(IUserRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get user by their id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var outPut = await _repo.GetById(userId);
            return Ok(new ApiResponse<UserResponse>
            {
                Data = outPut,
                Succeeded = true,
            });
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] string userId, [FromForm] UserRequest userReq)
        {
            return Ok(await _repo.UpdateUser(userId, userReq));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            return Ok(await _repo.DeleteUser(userId));
        }

        [HttpGet("searchuser-nojointeam")]
        public async Task<IActionResult> SearchUserNoJoinTeam(string teamId, string keyWord)
        {
            var outPut = await _repo.SearchUserNoJoinTeam(teamId, keyWord);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = outPut == null ? false : true,
            });
        }

        /// <summary>
        /// Get all user for tag
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("getuser-inteam")]
        public async Task<IActionResult> GetAllUserInTeam(string userId, string teamId = null)
        {
            var outPut = await _repo.GetAllUserInTeam(userId, teamId);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Succeeded = true,
                Data = outPut,
            });
        }

        [HttpGet("search-user")]
        public async Task<IActionResult> SearchUser([FromQuery] UserSearchModel searchModel)
        {
            var outPut = await _repo.SearchUser(searchModel.UserId, searchModel.Keyword, searchModel.IsEmail);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        [HttpGet("search-user-add-chat")]
        public async Task<IActionResult> SearchUserExistsAddChat([FromQuery] UserExistsChatAddModel searchModel)
        {
            var outPut = await _repo.SearchUserAddToExistsChat(searchModel.UserId, searchModel.GroupChatId, searchModel.Keyword, searchModel.IsEmail);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }
    }
}
