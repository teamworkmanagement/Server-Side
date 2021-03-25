using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces;
using TeamApp.Infrastructure.Persistence.Entities;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public TestController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult ShowTest()
        {
            return Ok("Okela");
        }

        [Authorize]
        [HttpGet("user-id")]
        public IActionResult GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return (Ok(userId));
        }
    }
}
