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
        /// Get user by user id API
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [ProducesDefaultResponseType(typeof(ApiResponse<UserResponse>))]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var outPut = await _repo.GetById(userId);
            return Ok(new ApiResponse<UserResponse>
            {
                Data = outPut,
                Succeeded = true,
            });
        }


        /*[HttpGet("searchuser-nojointeam")]
        public async Task<IActionResult> SearchUserNoJoinTeam(string teamId, string keyWord)
        {
            var outPut = await _repo.SearchUserNoJoinTeam(teamId, keyWord);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = outPut == null ? false : true,
            });
        }*/

        /// <summary>
        /// Get all user for tag API
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("getuser-inteam")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<UserResponse>>))]
        public async Task<IActionResult> GetAllUserInTeam(string userId, string teamId = null)
        {
            var outPut = await _repo.GetAllUserInTeam(userId, teamId);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Succeeded = true,
                Data = outPut,
            });
        }

        /// <summary>
        /// Search all users in all group user joined API
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet("search-user")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<UserResponse>>))]
        public async Task<IActionResult> SearchUser([FromQuery] UserSearchModel searchModel)
        {
            var outPut = await _repo.SearchUser(searchModel.UserId, searchModel.Keyword, searchModel.IsEmail);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        /// <summary>
        /// Search users to add to exists chat API
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet("search-user-add-chat")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<UserResponse>>))]
        public async Task<IActionResult> SearchUserExistsAddChat([FromQuery] UserExistsChatAddModel searchModel)
        {
            var outPut = await _repo.SearchUserAddToExistsChat(searchModel.UserId, searchModel.GroupChatId, searchModel.Keyword, searchModel.IsEmail);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        /// <summary>
        /// Search users for kanban action API
        /// </summary>
        /// <param name="userKanbanSearch"></param>
        /// <returns></returns>
        [HttpGet("search-users-kanban")]
        [ProducesDefaultResponseType(typeof(ApiResponse<List<UserResponse>>))]
        public async Task<IActionResult> SearchUsersKanban([FromQuery] UserKanbanSearchModel userKanbanSearch)
        {
            var outPut = await _repo.SearchUsersForKanban(userKanbanSearch);
            return Ok(new ApiResponse<List<UserResponse>>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }

        /// <summary>
        /// Update user avatar API
        /// </summary>
        /// <param name="updateImageModel"></param>
        /// <returns></returns>
        [HttpPatch("image")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UpdateUserImage(UpdateImageModel updateImageModel)
        {
            var outPut = await _repo.UpdateUserImage(updateImageModel);

            return (Ok(new ApiResponse<bool>
            {
                Data = outPut,
                Succeeded = outPut,
            }));
        }
    }
}
