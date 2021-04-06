using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Wrappers;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.WebApi.Export;
using TeamApp.WebApi.Extensions;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly IAuthenticatedUserService authenticatedUserService;
        private readonly TeamAppContext _dbContext;

        public TestController(IAuthenticatedUserService _authenticatedUserService, TeamAppContext dbContext)
        {
            authenticatedUserService = _authenticatedUserService;
            _dbContext = dbContext;
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
        [HttpPost("export-excel")]
        public async Task<IActionResult> GetExcel()
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "tests.xlsx";
            byte[] data = await ExportExcel.GenerateExcelFile(Data.Students);

            return File(data, contentType, fileName);
        }
        [HttpGet("convert-date")]
        public IActionResult ConvertDate(long timestamp)
        {
            return Ok(new
            {
                DateTime = Application.Utils.Extensions.UnixTimeStampToDateTime(timestamp),
            });
        }
        [HttpGet("test-time")]
        public IActionResult GetTime()
        {
            return Ok(DateTime.UtcNow);
        }

        [HttpGet("random")]
        public IActionResult Random()
        {
            return Ok(
                new ApiResponse<string>
                {
                    Data = RadomString.RandomString(6),
                    Succeeded = true,
                }
                );
        }
        [HttpGet("test-query")]
        public async Task<IActionResult> GetQuery([FromQuery] Request request)
        {
            //var outPut = from u in _dbContext.User
            //          where u.FullName == fullName && u.UserName == userName
            //         select u;
            var outPut = from m in _dbContext.Post
                         //where m.MessageCreatedAt >= request.StartDate && m.MessageCreatedAt <= request.EndDate
                         select m;
            return Ok(await outPut.ToListAsync());
        }

        public class Request
        {
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
