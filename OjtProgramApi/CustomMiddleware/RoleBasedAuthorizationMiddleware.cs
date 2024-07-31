using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OjtProgramApi.Models;
using OjtProgramApi.Repositories;

namespace OjtProgramApi.CustomMiddleware
{
    public class AuthorizeUserMiddleware : IMiddleware
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public AuthorizeUserMiddleware(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.Request.Path.ToString().ToLower();
            if (endpoint == "/api/user/login" || endpoint == "/api/publicrequest/heartbeat")
            {
                await next(context);
                return;
            }

            // Check if the user is authorized via policy-based authorization
            var authResult = await context.AuthenticateAsync();
            if (!authResult.Succeeded)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
           

            // Retrieve user ID  and role ID from claim
            int userID = int.Parse(context.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0");
            int roleID = int.Parse(context.User?.FindFirst(x => x.Type == "Roleid")?.Value ?? "0");
            User? user = await _repositoryWrapper.User.FindByID(userID);

            if (user != null && user.RoleID == roleID)
            {
                string permission = endpoint.Split("/").Last();

                RolePermission? rolePermission =
                    await _repositoryWrapper.RolePermission.GetRolePermission(roleID, permission);

                if (rolePermission != null)
                {
                    await next(context);
                }
                else
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden");
                }
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
    }

    public static class RequestAuthorizeUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizeUser(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizeUserMiddleware>();
        }
    }
}
