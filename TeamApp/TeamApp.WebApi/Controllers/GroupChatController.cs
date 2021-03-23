using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.GroupChat;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupChatController : ControllerBase
    {
        private readonly IGroupChatRepository _repo;
        public GroupChatController(IGroupChatRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var res = await _repo.GetAllByUserId(userId);

            var outPut = new ApiResponse<List<GroupChatResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupChat([FromForm] GroupChatRequest grChatReq)
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

        [HttpPut("{grChatId}")]
        public async Task<IActionResult> UpdateGroupChat(string grChatId, [FromForm] GroupChatRequest grChatReq)
        {
            var res = await _repo.UpdateGroupChat(grChatId, grChatReq);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res ? true : false,
                Message = !res ? "Upadte lỗi" : null,
            };

            return Ok(outPut);
        }

        [HttpDelete("{grChatId}")]
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
        }
    }
}
