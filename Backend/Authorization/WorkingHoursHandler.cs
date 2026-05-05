using Microsoft.AspNetCore.Authorization;

namespace Backend.Authorization;

public class WorkingHoursHandler : AuthorizationHandler<WorkingHoursRequirement>
{
    private readonly IConfiguration _configuration;

    public WorkingHoursHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkingHoursRequirement requirement)
    {
        if (!context.User.IsInRole("admin"))
        {
            return Task.CompletedTask;
        }

        int startHour = _configuration.GetValue<int>("ABACSettings:StartHour");
        int endHour = _configuration.GetValue<int>("ABACSettings:EndHour");

        int now = DateTime.Now.Hour;

        if (now >= startHour && now <= endHour)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}