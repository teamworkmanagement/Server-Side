using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Settings;

namespace TeamApp.Infrastructure.Persistence.Services
{
    public class AdminCheckRequirement : IAuthorizationRequirement
    {
        public AdminCheckRequirement()
        {

        }
    }

    public class AdminCheckHandler : AuthorizationHandler<AdminCheckRequirement>
    {
        public AdminCheckHandler(IHttpContextAccessor httpContextAccessor, IOptions<JWTSettings> jwtSettings)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _jwtSettings = jwtSettings.Value;
        }

        private IHttpContextAccessor HttpContextAccessor { get; }
        private readonly JWTSettings _jwtSettings;
        private HttpContext HttpContext => HttpContextAccessor.HttpContext;

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
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminCheckRequirement requirement)
        {
            Claim adminClaim = context.User.FindFirst(claim => claim.Type == "admin");
            //var value = adminClaim.Value;

            if (adminClaim != null && adminClaim.Value == "true")
            {
                context.Succeed(requirement);
            }

            else
            {
                context.Fail();
            }

            // No claim existing set and and its configured as optional so skip the check
            /*if (ipClaim == null && !requirement.IpClaimRequired)
            {
                // Optional claims (IsClaimRequired=false and no "ipaddress" in the claims principal) won't call context.Fail()
                // This allows next Handle to succeed. If we call Fail() the access will be denied, even if handlers
                // evaluated after this one do succeed
                return Task.CompletedTask;
            }*/

            /*var accessToken = HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if (accessToken == null)
            {
                return Task.CompletedTask;
            }
            if (GetPrincipalFromExpiredToken(accessToken).Claims.ToList()[4].Value == "true")
            {
                context.Succeed(requirement);
            }
            else
            {
                // Only call fail, to guarantee a failure, even if further handlers may succeed
                context.Fail();
            }*/

            return Task.CompletedTask;
        }
    }
}
