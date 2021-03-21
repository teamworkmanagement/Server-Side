using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Filters;
using TeamApp.Application.Interfaces.Repositories;

namespace TeamApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return Ok(await _repo.GetAllByUserId(userId));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] RequestParameter parameter)
        {
            return Ok(await _repo.GetPaging(parameter));
        }

        [HttpPost("{notiId}")]
        public async Task<IActionResult> ReadNotificationSet(string notiId)
        {
            return Ok(await _repo.ReadNotificationSet(notiId));
        }

        [HttpDelete("{notiId}")]
        public async Task<IActionResult> DeleteNotification(string notiId)
        {
            return Ok(await _repo.DeleteNotification(notiId));
        }
    }
}
