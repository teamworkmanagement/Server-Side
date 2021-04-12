using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamApp.Infrastructure.Persistence.Helpers;

namespace TeamApp.WebApi.Middlewares
{
    public class CookieMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var tokenEncry = context.Request.Cookies["access_token"];
            var backupCookie = context.Request.Cookies["backup"];
            bool hasAuthorizeAttribute = false;
            if (context.Features.Get<IEndpointFeature>().Endpoint != null)
                hasAuthorizeAttribute = context.Features.Get<IEndpointFeature>().Endpoint.Metadata
                    .Any(m => m is AuthorizeAttribute);

            if (!string.IsNullOrEmpty(backupCookie) && !string.IsNullOrEmpty(tokenEncry) && string.IsNullOrEmpty(context.Request.Headers["Authorization"]) && hasAuthorizeAttribute)
            {
                var token = StringHelper.DecryptString(tokenEncry);
                context.Request.Headers.Add("Authorization", "Bearer " + token);
            }

            await _next(context);
        }
    }
}
