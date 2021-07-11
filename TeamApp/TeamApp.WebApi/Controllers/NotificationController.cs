using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _repo;
        public NotificationController(INotificationRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get pagination noti data API
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(ApiResponse<PagedResponse<NotificationResponse>>))]
        public async Task<IActionResult> GetPaging([FromQuery] NotificationRequestParameter parameter)
        {
            var res = await _repo.GetPaging(parameter);

            var outPut = new ApiResponse<PagedResponse<NotificationResponse>>
            {
                Data = res,
                Succeeded = true,
            };

            return Ok(outPut);
        }

        /// <summary>
        /// Click noti API
        /// </summary>
        /// <param name="readNotiModel"></param>
        /// <returns></returns>
        [HttpPost("read-noti")]
        [ProducesDefaultResponseType(typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ReadNotificationSet([FromBody]ReadNotiModel readNotiModel)
        {
            var res = await _repo.ReadNotificationSet(readNotiModel);

            var outPut = new ApiResponse<bool>
            {
                Data = res,
                Succeeded = res,
            };

            return Ok(outPut);
        }
    }
}
