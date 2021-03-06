using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Account;
using TeamApp.Application.Wrappers;

namespace TeamApp.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
        Task<ApiResponse<string>> RegisterAsync(RegisterRequest request, string origin);
        Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string code, string apiOrigin);
        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest model);
        Task<ApiResponse<TokenModel>> Refresh(string refreshTokenEncry);
        Task<ApiResponse<AuthenticationResponse>> SocialLogin(SocialRequest request);
        Task<ApiResponse<string>> IsLogin(string accessToken, string refreshToken);
        Task<ApiResponse<bool>> ChangePassword(ChangePasswordModel changePasswordModel);
        Task<ApiResponse<bool>> UpdateUserInfo(UpdateInfoModel updateInfoModel);
    }
}
