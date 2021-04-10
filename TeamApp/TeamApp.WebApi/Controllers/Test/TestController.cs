using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Wrappers;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Infrastructure.Persistence.Helpers;
using TeamApp.WebApi.Export;
using TeamApp.WebApi.Extensions;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Route("api/test")]
    public class TestController : Controller
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
        public async Task<IActionResult> GetQuery()
        {
            /*HttpContext.Response.Cookies.Append("test", "bánh quy", new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(2)
            });*/
            HttpContext.Request.Cookies.TryGetValue("test", out string z);
            return Ok("abc query: " + z);
        }

        [AllowAnonymous]
        [HttpGet("decrypt")]
        public IActionResult Decrypt(string encry, bool isDecode = false)
        {
            if (!isDecode)
                encry = HttpUtility.UrlDecode(encry);
            return Ok(StringHelper.DecryptString(encry));
        }
    }
}
