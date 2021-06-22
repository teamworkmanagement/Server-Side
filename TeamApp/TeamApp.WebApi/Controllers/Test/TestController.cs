using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using TeamApp.Application.DTOs.Email;
using TeamApp.Application.DTOs.Team;
using TeamApp.Application.Exceptions;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Wrappers;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Infrastructure.Persistence.Helpers;
using TeamApp.WebApi.Export;
using TeamApp.WebApi.Extensions;
using static TeamApp.Application.Utils.Extensions;
using RadomString = TeamApp.Application.Utils.Extensions.RadomString;

namespace TeamApp.WebApi.Controllers.Test
{
    [ApiController]
    [Route("api/test")]
    public class TestController : Controller
    {
        private readonly IAuthenticatedUserService authenticatedUserService;
        private readonly TeamAppContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;

        public TestController(IAuthenticatedUserService _authenticatedUserService, TeamAppContext dbContext, IWebHostEnvironment environment, IEmailService emailService)
        {
            authenticatedUserService = _authenticatedUserService;
            _dbContext = dbContext;
            _environment = environment;
            _emailService = emailService;
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
        [HttpGet("export-excel")]
        public async Task<FileContentResult> GetExcel()
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
        public async Task<IActionResult> Random()
        {
            var res = await _dbContext.Message.FindAsync("2dc73c78-c510-4387-91e2-3725ffb3178c");
            return Ok(
                new ApiResponse<DateTime?>
                {
                    Data = res.MessageCreatedAt.FormatTime(),
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

        [HttpGet("send-mail")]
        public async Task<IActionResult> SendMailAsync()
        {
            var obj = new EmailRequest
            {
                To = "dunghkuit@gmail.com",
                Subject = "test mail",
                Body = "lorem lorem lorem"
            };
            await _emailService.SendAsyncAWS(obj);

            return Ok("zzzzzzzzzz");
        }

        [Authorize(Policy = "TeamPolicy")]
        [HttpGet("test-policy")]
        public IActionResult TestPolicy([FromQuery] TeamResponse teamResponse)
        {
            return Ok("zzzzzzzz");
        }

        [Authorize(Policy = "TeamPolicy")]
        [HttpPost("test-policy2")]
        public IActionResult TestPolicy2([FromBody] TeamResponse teamResponse)
        {
            return Ok("zzzzzzzz");
        }

        [HttpGet("listdoublestring")]
        public IActionResult RandomText(int number)
        {
            var rd = new Random();
            List<int> lengths = new List<int> { 6, 7, 8, 9, 10 };
            var results = new List<object>();
            for (int i = 0; i < number; i++)
            {
                var a = RadomString.RandomString(lengths[rd.Next(lengths.Count)]);
                var b = RadomString.RandomString(lengths[rd.Next(lengths.Count)]);

                var check = string.Compare(a, b) < 0;
                results.Add(new
                {
                    a = check ? a : b,
                    b = check ? b : a,
                });
            }

            var obj = new
            {
                Counts = number,
                Data = results,
            };

            return Content(JsonConvert.SerializeObject(obj), "application/json");

        }
    }
}
