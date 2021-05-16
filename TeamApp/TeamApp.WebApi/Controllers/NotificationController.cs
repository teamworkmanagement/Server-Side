﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
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

        [HttpPost("push")]
        public async Task<IActionResult> PushNoti(List<string> userIds, string title, string body)
        {
            await _repo.PushNoti(userIds, title, body);
            return Ok();
        }
    }
}
