using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Notification;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Application.Wrappers;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _repo;
        public NotificationController(INotificationRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var res = await _repo.GetAllByUserId(userId);

            var outPut = new ApiResponse<List<NotificationResponse>>
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

            var outPut = new ApiResponse<PagedResponse<NotificationResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        [HttpPost("{notiId}")]
        public async Task<IActionResult> ReadNotificationSet(string notiId)
        {
            var res = await _repo.ReadNotificationSet(notiId);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
            };

            return Ok(outPut);
        }

        [HttpDelete("{notiId}")]
        public async Task<IActionResult> DeleteNotification(string notiId)
        {
            var res = await _repo.DeleteNotification(notiId);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
            };

            return Ok(outPut);
        }
    }
}
