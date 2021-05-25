using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
using TeamApp.Infrastructure.Persistence.Entities;
using Task = System.Threading.Tasks.Task;

namespace TeamApp.Infrastructure.Persistence.Services
{
    public class TeamCheckRequirement : IAuthorizationRequirement
    {
        public TeamCheckRequirement()
        {

        }
    }
    public class TeamCheckHandler : AuthorizationHandler<TeamCheckRequirement>
    {
        public TeamCheckHandler(IHttpContextAccessor httpContextAccessor, IOptions<JWTSettings> jwtSettings, IServiceProvider serviceProvider)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _jwtSettings = jwtSettings.Value;
            _serviceProvider = serviceProvider;
        }

        private IHttpContextAccessor HttpContextAccessor { get; }
        private readonly JWTSettings _jwtSettings;
        private readonly IServiceProvider _serviceProvider;
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
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TeamCheckRequirement requirement)
        {
            Claim ipClaim = context.User.FindFirst(claim => claim.Type == "uid");

            // No claim existing set and and its configured as optional so skip the check
            /*if (ipClaim == null && !requirement.IpClaimRequired)
            {
                // Optional claims (IsClaimRequired=false and no "ipaddress" in the claims principal) won't call context.Fail()
                // This allows next Handle to succeed. If we call Fail() the access will be denied, even if handlers
                // evaluated after this one do succeed
                return Task.CompletedTask;
            }*/
            var body = HttpContext.Request.Body;
            var accessToken = HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if (accessToken == null)
            {
                return;
            }

            var zzz = GetPrincipalFromExpiredToken(accessToken).Claims.ToList()[3].Value;
            var abc = HttpContext.Request.Query["teamId"].FirstOrDefault();

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TeamAppContext>();

                var par =await (from p in dbContext.Participation.AsNoTracking()
                          where p.ParticipationUserId == zzz && p.ParticipationTeamId == abc
                          select p).FirstOrDefaultAsync();
               
                if (par != null)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }






            /*{
                // Only call fail, to guarantee a failure, even if further handlers may succeed
                
            }*/

            return;
        }
    }
}
