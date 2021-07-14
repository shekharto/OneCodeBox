using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Helpers;
using UserManagement.JWT.Interface;

namespace UserManagement.JWT.Authorization
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext httpContext, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateJwtToken(token);
            if (userId != null)
            {
                // attach user to context on successfull jwt validation
                httpContext.Items["JwtUser"] = userService.GetById(userId.Value);
            }
            //  return _next(httpContext);
            await _next(httpContext);

        }
    }

    //// Extension method used to add the middleware to the HTTP request pipeline.
    //public static class JwtMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<JwtMiddleware>();
    //    }
    //}
}
