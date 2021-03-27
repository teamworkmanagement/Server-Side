using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IAuthenticatedUserService authenticatedUserService;

        public TestController(IAuthenticatedUserService _authenticatedUserService)
        {
            authenticatedUserService = _authenticatedUserService;
        }
        [Authorize]
        [HttpGet]
        public IActionResult ShowTest()
        {
            return Ok(new
            {
                Text = "Okela",
            });
        }

        //[Authorize]
        [HttpGet("userId")]
        public IActionResult GetUserId()
        {
            return Ok(
                new
                {
                    UserId = authenticatedUserService.UserId,
                });
        }
    }
}
