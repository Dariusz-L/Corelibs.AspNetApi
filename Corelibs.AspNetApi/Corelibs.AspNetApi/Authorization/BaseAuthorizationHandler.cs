using Microsoft.AspNetCore.Authorization;

namespace Corelibs.AspNetApi.Authorization
{
    public abstract class BaseAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement>
        where TRequirement : IAuthorizationRequirement
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TRequirement requirement)
        {
            bool result = await HandleRequirementImplAsync(context, requirement);
            if (result)
                context.Succeed(requirement);
            else
                context.Fail();
        }

        protected abstract Task<bool> HandleRequirementImplAsync(
            AuthorizationHandlerContext context,
            TRequirement requirement);
    }
}
