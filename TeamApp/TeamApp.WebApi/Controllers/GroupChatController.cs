using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.GroupChat;
using TeamApp.Application.Exceptions;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/groupchat")]
    public class GroupChatController : ControllerBase
    {
        private readonly IGroupChatRepository _repo;
        public GroupChatController(IGroupChatRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get group chat by user id API
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("byuserid")]
        [ProducesResponseType(typeof(ApiResponse<CustomListGroupChatResponse>), 200)]
        public async Task<IActionResult> GetAllByUserId([FromQuery] GroupChatSearch search)
        {
            var res = await _repo.GetAllByUserId(search);

            var outPut = new ApiResponse<CustomListGroupChatResponse>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Add group chat API
        /// </summary>
        /// <param name="grChatReq"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> AddGroupChat([FromBody] GroupChatRequest grChatReq)
        {
            var res = await _repo.AddGroupChat(grChatReq);

            var outPut = new ApiResponse<string>
            {
                Data = res == null ? null : res,
                Succeeded = res == null ? false : true,
                Message = res == null ? "Lỗi khi thêm" : null,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Change group chat image API
        /// </summary>
        /// <param name="grChatReq"></param>
        /// <returns></returns>
        [HttpPatch("image")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> UpdateGroupChatImageUrl(GroupChatImageUpdateRequest grChatReq)
        {
            var res = await _repo.UpdateGroupChatImageUrl(grChatReq);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res ? true : false,
                Message = !res ? "Upadte lỗi" : null,
            };

            return Ok(outPut);
        }

        /*[HttpDelete("{grChatId}")]
        public async Task<IActionResult> DeleteGroupChat(string grChatId)
        {
            var res = await _repo.DeleteGroupChat(grChatId);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res ? true : false,
                Message = !res ? "Xoas lỗi" : null,
            };

            return Ok(outPut);
        }*/

        /// <summary>
        /// Check exists chat 1 vs 1 API
        /// </summary>
        /// <param name="chatExists"></param>
        /// <returns></returns>
        [HttpPost("check-double-exists")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> CheckDoubleExists([FromBody] CheckDoubleGroupChatExists chatExists)
        {
            var res = await _repo.CheckDoubleGroupChatExists(chatExists);

            var outPut = new ApiResponse<object>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Create group chat with members list API
        /// </summary>
        /// <param name="requestMembers"></param>
        /// <returns></returns>
        [HttpPost("add-with-members")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> AddGroupChatWithMembers(GroupChatRequestMembers requestMembers)
        {
            var outPut = await _repo.AddGroupChatWithMembers(requestMembers);
            return Ok(new ApiResponse<string>
            {
                Succeeded = true,
                Data = outPut,
            });
        }

        /// <summary>
        /// Add group chat members API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add-members")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> AddGroupChatMembers(AddMembersRequest request)
        {
            var outPut = await _repo.AddGroupChatMembers(request);
            return Ok(new ApiResponse<string>
            {
                Data = outPut,
                Succeeded = outPut != null
            });
        }
    }
}
