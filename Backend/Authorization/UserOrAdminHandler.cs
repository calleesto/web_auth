using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Backend.Authorization;

public class UserOrAdminHandler : AuthorizationHandler<UserOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOrAdminRequirement requirement)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            return Task.CompletedTask;
        }

        if (context.User.IsInRole("admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        Claim? userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        string? routeId = httpContext.Request.RouteValues["id"]?.ToString();

        if (routeId == null)
        {
            return Task.CompletedTask;
        }

        if (userIdClaim.Value == routeId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}