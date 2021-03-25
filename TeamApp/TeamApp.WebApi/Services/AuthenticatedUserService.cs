using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces;

namespace TeamApp.WebApi.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public AuthenticatedUserService(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
            //UserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public string UserId { get; }
    }
}
