using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Account;
using TeamApp.Application.Interfaces;

namespace TeamApp.WebApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            var outPut = await _accountService.AuthenticateAsync(request, GenerateIPAddress());
            if (outPut.Succeeded)
            {
                var data = outPut.Data;

                HttpContext.Response.Cookies.Append("access_token", data.JWToken,
                    new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddMinutes(330), });
                HttpContext.Response.Cookies.Append("refresh_token", data.RefreshToken,
                    new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(7), });
            }

            return Ok(outPut);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _accountService.RegisterAsync(request, origin));
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _accountService.ConfirmEmailAsync(userId, code));
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _accountService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok();
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {

            return Ok(await _accountService.ResetPassword(model));
        }
        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshTokenEncry)
        {
            var outPut = await _accountService.Refresh(refreshTokenEncry);
            if (outPut.Succeeded)
            {
                HttpContext.Response.Cookies.Append("access_token", outPut.Data.AccessToken,
                    new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddMinutes(330), });
            }
            return Ok(outPut);
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> LoginSocial([FromBody] SocialRequest request)
        {
            var outPut = await _accountService.SocialLogin(request, GenerateIPAddress());
            if (outPut.Succeeded)
            {
                HttpContext.Response.Cookies.Append("access_token", outPut.Data.JWToken,
                    new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddMinutes(330), });
            }

            return Ok(outPut);
        }
    }
}