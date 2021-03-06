using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Account;
using TeamApp.Application.Exceptions;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Settings;
using TeamApp.Infrastructure.Persistence.Helpers;

namespace TeamApp.WebApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<MyAppSettings> _config;
        public AccountController(IAccountService accountService, IHttpContextAccessor httpContextAccessor, IOptions<MyAppSettings> config)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }
        /// <summary>
        /// Login API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), 200)]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            var outPut = await _accountService.AuthenticateAsync(request);
            if (outPut.Succeeded)
            {
                var data = outPut.Data;

                HttpContext.Response.Cookies.Append("access_token", data.JWToken,
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(30), });
                HttpContext.Response.Cookies.Append("refresh_token", data.RefreshToken,
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(7), });
                HttpContext.Response.Cookies.Append("backup", Guid.NewGuid().ToString(),
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = false, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(30), });
                data.JWToken = null;
                data.RefreshToken = null;
            }

            return Ok(outPut);
        }

        /// <summary>
        /// Register API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers["origin"];
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.Value;

            //https://localhost:9001

            var apiOrigin = string.Concat(scheme, "://", host);
            return Ok(await _accountService.RegisterAsync(request, apiOrigin));
        }
        /// <summary>
        /// Confirm email when register API
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("confirm-email")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            var origin = Request.Headers["origin"];
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.Value;

            //https://localhost:9001

            var apiOrigin = string.Concat(scheme, "://", host);
            return Ok(await _accountService.ConfirmEmailAsync(userId, code, apiOrigin));
        }

        /// <summary>
        /// Reset password API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _accountService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok();
        }

        /// <summary>
        /// Reset Password API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            var origin = Request.Headers["origin"];
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.Value;

            //https://localhost:9001

            var apiOrigin = string.Concat(scheme, "://", host);
            return Ok(await _accountService.ResetPassword(model));
        }

        /// <summary>
        /// Refresh access token API
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<TokenModel>), 200)]
        public async Task<IActionResult> Refresh()
        {
            var refreshEncry = HttpContext.Request.Cookies["refresh_token"];
            var accessToken = HttpContext.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(refreshEncry) || (string.IsNullOrEmpty(accessToken)))
                throw new ArgumentException();

            var outPut = await _accountService.Refresh(refreshEncry);
            if (outPut.Succeeded)
            {
                HttpContext.Response.Cookies.Append("access_token", outPut.Data.AccessToken,
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(30), });
                HttpContext.Response.Cookies.Append("refresh_token", outPut.Data.RefreshToken,
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(7), });
                HttpContext.Response.Cookies.Append("backup", Guid.NewGuid().ToString(),
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = false, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(30), });
                outPut.Data.RefreshToken = null;
                outPut.Data.AccessToken = null;
            }
            return Ok(outPut);
        }

        /// <summary>
        /// Social login API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("social-login")]
        [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), 200)]
        public async Task<IActionResult> LoginSocial([FromBody] SocialRequest request)
        {
             var outPut = await _accountService.SocialLogin(request);
            if (outPut.Succeeded)
            {
                HttpContext.Response.Cookies.Append("access_token", outPut.Data.JWToken,
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(30), });
                HttpContext.Response.Cookies.Append("refresh_token", outPut.Data.RefreshToken,
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(7), });
                HttpContext.Response.Cookies.Append("backup", Guid.NewGuid().ToString(),
                    new CookieOptions { Domain = _config.Value.Url, Secure = true, HttpOnly = false, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(30), });
            }

            return Ok(outPut);
        }

        /// <summary>
        /// Check login API
        /// </summary>
        /// <returns></returns>
        [HttpGet("is-login")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> IsLogin()
        {
            var token = HttpContext.Request.Cookies["access_token"];
            var refreshtoken = HttpContext.Request.Cookies["refresh_token"];
            var backup = HttpContext.Request.Cookies["backup"];
            if (string.IsNullOrEmpty(backup))
                throw new ApiException("Đã logout");

            return Ok(await _accountService.IsLogin(token, refreshtoken));
        }

        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var outPut = await _accountService.ChangePassword(model);

            return Ok(outPut);
        }

        [HttpPatch("update-info")]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateInfoModel infoModel)
        {
            var outPut = await _accountService.UpdateUserInfo(infoModel);
            return Ok(outPut);
        }
    }
}