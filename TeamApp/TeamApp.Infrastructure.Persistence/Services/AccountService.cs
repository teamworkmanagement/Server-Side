using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TeamApp.Application.DTOs.Account;
using TeamApp.Application.DTOs.Email;
using TeamApp.Application.Exceptions;
using TeamApp.Application.Interfaces;
using TeamApp.Application.Wrappers;
using TeamApp.Domain.Settings;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Infrastructure.Persistence.Helpers;
using Task = System.Threading.Tasks.Task;

namespace TeamApp.Infrastructure.Persistence.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;
        private readonly JWTSettings _jwtSettings;
        private readonly TeamAppContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(UserManager<User> userManager,
            IOptions<JWTSettings> jwtSettings,
            SignInManager<User> signInManager,
            IEmailService emailService,
            TeamAppContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _emailService = emailService;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.Email}.");
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.Email}'.");
            }
            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id;
            response.JWToken = StringHelper.EncryptString(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
            response.Email = user.Email;
            response.UserName = user.UserName;
            response.UserDob = user.Dob;
            response.UserPhoneNumber = user.PhoneNumber;

            //response.IsVerified = user.EmailConfirmed;
            response.UserAvatar = string.IsNullOrEmpty(user.ImageUrl) ? $"https://ui-avatars.com/api/?name={user.FullName}" : user.ImageUrl;
            response.FullName = user.FullName;

            var refreshToken = GenerateRefreshToken(IpHelper.GetIpAddress());
            refreshToken.UserId = user.Id;
            response.RefreshToken = StringHelper.EncryptString(refreshToken.Token);
            response.ExprireToken = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes)).ToUnixTimeMilliseconds();

            await _dbContext.RefreshToken.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new ApiResponse<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                throw new ApiException($"Username '{request.Email}' is already taken.");
            }
            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                UserName = request.Email,
                CreatedAt = DateTime.UtcNow,
            };

            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                    var verificationUri = await SendVerificationEmail(user, origin);
                    //TODO: Attach Email Service here and configure it via appsettings
                    await _emailService.SendAsyncAWS(new Application.DTOs.Email.EmailRequest() { From = "KD", To = user.Email, Body = $"Hãy xác thực tài khoản của bạn bằng việc nhấn vào link dưới <br> <a href=\'{verificationUri}\'>Xác thực</a>", Subject = "Confirm Registration" });
                    return new ApiResponse<string>(user.Id, message: $"User Registered. Please confirm your account by visiting this URL {verificationUri}");
                }
                else
                {
                    var errStr = "";
                    foreach (var e in result.Errors)
                    {
                        errStr += e.Description;
                    }
                    throw new ApiException($"{errStr}");
                }
            }
            else
            {
                throw new ApiException($"Email {request.Email } is already registered.");
            }
        }

        private async Task<JwtSecurityToken> GenerateJWToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private async Task<string> SendVerificationEmail(User user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/account/confirm-email/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }

        public async Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return new ApiResponse<string>(user.Id, message: $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
            }
            else
            {
                throw new ApiException($"An error occured while confirming {user.Email}.");
            }
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);
            var route = "api/account/reset-password/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var emailRequest = new EmailRequest()
            {
                Body = $"You reset token is - {code}",
                To = model.Email,
                Subject = "Reset Password",
            };
            await _emailService.SendAsyncAWS(emailRequest);
        }

        public async Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null) throw new ApiException($"No Accounts Registered with {model.Email}.");
            var result = await _userManager.ResetPasswordAsync(account, model.Token, model.Password);

            if (result.Succeeded)
            {
                return new ApiResponse<string>(model.Email, message: $"Password Resetted.");
            }
            else
            {
                throw new ApiException($"Error occured while reseting the password.");
            }
        }

        public async Task<ApiResponse<TokenModel>> Refresh(string refreshTokenEncry)
        {
            var refreshToken = StringHelper.DecryptString(refreshTokenEncry);
            var accessTokenEncry = _httpContextAccessor.HttpContext.Request.Cookies["access_token"];

            var accesToken = StringHelper.DecryptString(accessTokenEncry);
            var principal = GetPrincipalFromExpiredToken(accesToken);

            var userId = principal.Claims.ToList()[1].Value;
            var user = await _dbContext.User.FindAsync(userId);

            var tokenObj = await _dbContext.RefreshToken.Where(x => x.UserId == userId
            && x.Token == refreshToken && x.Expires > DateTime.UtcNow).FirstOrDefaultAsync();

            if (tokenObj == null)
                throw new ApiException("Refresh token not match for this user");

            var jwtSecurityToken = await GenerateJWToken(user);
            var refreshObj = GenerateRefreshToken(IpHelper.GetIpAddress());
            refreshObj.UserId = userId;

            await _dbContext.RefreshToken.AddAsync(refreshObj);
            await _dbContext.SaveChangesAsync();

            //_dbContext.RefreshToken.Remove(tokenObj);
            //await _dbContext.SaveChangesAsync();

            var outPut = new ApiResponse<TokenModel>
            {
                Succeeded = true,
                Data = new TokenModel
                {
                    RefreshToken = StringHelper.EncryptString(refreshObj.Token),
                    AccessToken = StringHelper.EncryptString(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)),
                    ExprieToken = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(359)).ToUnixTimeMilliseconds(),
                }
            };

            return outPut;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public async Task<ApiResponse<AuthenticationResponse>> SocialLogin(SocialRequest request, string ipAddress)
        {
            AuthenticationResponse response = new AuthenticationResponse();
            var userWithSameEmail = await _dbContext.User.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
            //đã đăng nhập với social
            if (userWithSameEmail != null)
            {
                //đã login trước, cấp token
                JwtSecurityToken jwtSecurityToken = await GenerateJWToken(userWithSameEmail);

                response.Id = userWithSameEmail.Id;
                response.JWToken = StringHelper.EncryptString(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
                response.Email = userWithSameEmail.Email;
                response.UserName = userWithSameEmail.UserName;
                response.UserAvatar = userWithSameEmail.ImageUrl;
                response.FullName = userWithSameEmail.FullName;
                response.IsVerified = userWithSameEmail.EmailConfirmed;

                var refreshToken = GenerateRefreshToken(IpHelper.GetIpAddress());
                refreshToken.UserId = userWithSameEmail.Id;
                response.RefreshToken = StringHelper.EncryptString(refreshToken.Token);
                response.ExprireToken = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(359)).ToUnixTimeMilliseconds();

                await _dbContext.RefreshToken.AddAsync(refreshToken);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var user = new User
                {
                    Id = request.Id.ToString(),
                    Email = request.Email,
                    FullName = request.FullName,
                    ImageUrl = HttpUtility.UrlEncode(request.ImageUrl),
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true,
                    UserName = request.Email,
                };

                var result = await _userManager.CreateAsync(user, "@Social11");

                var entity = await _userManager.FindByIdAsync(request.Id);

                JwtSecurityToken jwtSecurityToken = await GenerateJWToken(entity);
                response.Id = entity.Id;
                response.JWToken = StringHelper.EncryptString(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
                response.Email = entity.Email;
                response.IsVerified = entity.EmailConfirmed;
                response.UserAvatar = entity.ImageUrl;
                response.FullName = entity.FullName;

                var refreshToken = GenerateRefreshToken(IpHelper.GetIpAddress());
                refreshToken.UserId = entity.Id;
                response.RefreshToken = StringHelper.EncryptString(refreshToken.Token);
                response.ExprireToken = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(359)).ToUnixTimeMilliseconds();

                await _dbContext.RefreshToken.AddAsync(refreshToken);
                await _dbContext.SaveChangesAsync();
            }

            return new ApiResponse<AuthenticationResponse>(response);

        }

        public async Task<ApiResponse<string>> IsLogin(string accessToken, string refreshToken)
        {
            var outPut = "Auth";

            //không có token trong cookie
            if (string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
                outPut = "UnAuth";

            if (!string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
                outPut = "UnAuth";

            if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                outPut = "UnAuth";
            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var principal = GetPrincipalFromExpiredToken(StringHelper.DecryptString(accessToken)).Claims.ToList();
                var userId = principal[3].Value;
                var refreshDec = StringHelper.DecryptString(refreshToken);
                var refreshEntity = await _dbContext.RefreshToken.Where(x => x.UserId == userId && x.Token == refreshDec).FirstOrDefaultAsync();
                if (refreshEntity == null)
                    outPut = "UnAuth";
            }
            return new ApiResponse<string>
            {
                Succeeded = true,
                Data = outPut,
                Message = null,
            };
        }

        public async Task<ApiResponse<bool>> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            var user = await _dbContext.User.FindAsync(changePasswordModel.UserId);
            if (user == null)
                throw new KeyNotFoundException("No user found");

            var results = await _userManager.ChangePasswordAsync(user, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);

            if (results.Succeeded)
            {
                return new ApiResponse<bool>
                {
                    Data = true,
                    Succeeded = true,
                    Message = "Change password done",
                };
            }

            throw new ApiException("Errors while change");
        }

        public async Task<ApiResponse<bool>> UpdateUserInfo(UpdateInfoModel updateInfoModel)
        {
            var user = await _dbContext.User.FindAsync(updateInfoModel.Id);
            if (user == null)
                throw new KeyNotFoundException("No user found");
            if (user.Email != updateInfoModel.Email)
            {
                var userSameEmail = await _userManager.FindByEmailAsync(updateInfoModel.Email);
                if (userSameEmail != null)
                    throw new AlreadyExistsException("Mail Exists");
            }


            var token = await _userManager.GenerateChangeEmailTokenAsync(user, updateInfoModel.Email);
            var results = await _userManager.ChangeEmailAsync(user, updateInfoModel.Email, token);




            if (results.Succeeded)
            {
                user = await _userManager.FindByEmailAsync(updateInfoModel.Email);
                user.FullName = updateInfoModel.FullName;
                user.Dob = updateInfoModel.UserDob;
                user.PhoneNumber = updateInfoModel.UserPhoneNumber;
                user.UserName = updateInfoModel.Email;

                await _userManager.UpdateAsync(user);
                return new ApiResponse<bool>
                {
                    Data = true,
                    Succeeded = true,
                    Message = "Update user done",
                };
            }

            throw new ApiException("Errors while change");
        }
    }
}
