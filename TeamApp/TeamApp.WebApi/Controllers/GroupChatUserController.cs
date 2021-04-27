using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.GroupChatUser;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/groupchatuser")]
    public class GroupChatUserController : ControllerBase
    {
        private readonly IGroupChatUserRepository _repo;
        public GroupChatUserController(IGroupChatUserRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupChatUser([FromForm] GroupChatUserRequest grChatUserReq)
        {
            var res = await _repo.AddGroupChatUser(grChatUserReq);


            return Ok(new ApiResponse<string>
            {
                Succeeded = res == null ? false : true,
                Message = res == null ? "Thêm không thành công" : null,
                Data = res,
            });
        }

        [HttpDelete("group/{groupId}/user/{userId}")]
        public async Task<IActionResult> DeleteUserInGroup(string groupId, string userId)
        {
            var res = await _repo.DeleteGroupChatUser(groupId, userId);

            return Ok(new ApiResponse<bool>
            {
                Succeeded = res,
                Message = res ? "Xóa thành công" : "Xóa thất bại",
                Data = res,
            });
        }

        [HttpPut("toggle-seen")]
        public async Task<IActionResult> ToggleSeen(GroupChatUserSeenRequest request)
        {
            var outPut = await _repo.ToggleSeen(request);

            return Ok(new ApiResponse<bool>
            {
                Succeeded = outPut,
                Data = outPut,
                Message = outPut ? "Cập nhật thành công" : "Cập nhật thất bại",
            });
        }
    }
}
