using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using TeamApp.Application.Exceptions;
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
        private readonly IWebHostEnvironment _environment;

        public TestController(IAuthenticatedUserService _authenticatedUserService, TeamAppContext dbContext, IWebHostEnvironment environment)
        {
            authenticatedUserService = _authenticatedUserService;
            _dbContext = dbContext;
            _environment = environment;
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

        [HttpGet("cookie-chrome")]
        public IActionResult Cookie(bool check = false)
        {
            var value = "value";
            if (check)
                value = "CfDJ8Ow0LkUrqFdFuyvzQuWx8xMeVTPT_Qmwf40WdCw4SOOYx7jV_KbK6lnAqIbsguLDypCcOOa-2BbPFJpZhVIvDCUFoIpkE3M-u1oFB6bGx8ZqdKLgdd0WTRRSUyXtd0kCRc3UwCh08bZ1YgUObrSKO64MKJY4ntq-XbgJ7HpdBTLmP3ifdDABrMvazMYUCBocZ5-55wMXkSisvwSva6fziluLphg1f6J7GRx_XVVsiVYVihqB_BR6Ynhl4U_qtW6v6yx8oACAAjW7uiM4xzo3z_2rPiU3Scp4bPV9wqTaEGh8z6Z05OSir8ZUWaP3N5RjO-3xRATydslEGPTO6Cq3Yq5gJOGQ3_2UWRwCHldIEoF6_Y0HNxDTfux6XTUyuRdHUQPVYKuQzWy7aCPchwaRNn0qVh5rEYZJBSt8u2unqRIWC9qwrb6kK_6ql9NSyHd72tkZbRPeWNF1rnmWMhal3mM8PUx5-qbzrFgbdpoxY7E0BGTdYMS7UJoFofyTgR0yAfwL-8SdsH8PZ7TyfWiRRQmH4yagYmOOE5uv3QqSFwPCD5F8cVISRkvf8Ym5t64GtQmxXmzogcfVkesuVmYNGIvw-rpK_LLVP1Vllw4MeNAl__lIwBIdSJUyTU03I19ZeQfVkesuVmYNGIvw-rpK_LLVP1Vllw4MeNAl__lIwBIdSJUyTU03I19ZeQ";

            HttpContext.Response.Cookies.Append(Guid.NewGuid().ToString(), value,
                    new CookieOptions { Secure = true, HttpOnly = false, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(30), });

            return Ok("niceeeee");
        }

        [HttpGet("getmes")]
        public async Task<IActionResult> GetMes()
        {
            var mes = await _dbContext.Message.FirstOrDefaultAsync();
            var a = DateTimeFormatInfo.CurrentInfo.Calendar;
            var zzzzzz = DateTime.UtcNow.Date;
            var time = ((DateTime)mes.MessageCreatedAt).ToLocalTime();
            return Ok(
                new ApiResponse<object>
                {
                    Succeeded = true,
                    Data = mes,
                }
            );
        }

        [HttpPost("upload-file")]
        public IActionResult UpLoadFile(IFormFile file)
        {
            if (file.Length > 0)
            {
                var folder = Guid.NewGuid().ToString();
                Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\" + folder);


                using (FileStream fs = System.IO.File.Create(_environment.WebRootPath +
                    "\\Upload\\" + folder + "\\" + file.FileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            return Ok(file.FileName);
        }
    }
}
