using Common.Basic.Blocks;
using Common.Basic.Collections;
using Common.Basic.DDD;
using Common.Basic.Repository;
using Corelibs.Basic.Architecture.DDD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Corelibs.AspNetApi.Authorization
{
    public class ResourceAuthorizationHandler<TResource> : AuthorizationHandler<SameOwnerRequirement<TResource>>
        where TResource : IEntity, IOwnedEntity
    {
        private readonly IServiceProvider _serviceProvider;
        private  IRepository<TResource> _resourceRespository;

        public ResourceAuthorizationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SameOwnerRequirement<TResource> requirement)
        {
            var userID = context.User.GetUserID();
            if (userID.IsNullOrEmpty())
            {
                context.Fail();
                return;
            }
            
            if (context.Resource is not HttpContext httpContext)
            {
                context.Fail();
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                _resourceRespository = scope.ServiceProvider.GetRequiredService<IRepository<TResource>>();

                var routeValues = httpContext.GetRouteData().Values;
                var routeValue = routeValues.LastOrDefault(kv => kv.Key.ToLower().Contains("id"));
                if (routeValue.Key.IsNullOrEmpty() || (routeValue.Value as string).IsNullOrEmpty())
                {
                    context.Fail();
                    return;
                }

                var resourceID = routeValue.Value as string;

                var result = new Result();
                var resource = await _resourceRespository.Get(resourceID, result);
                
                if (userID == resource.OwnerID)
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
        }
    }

    public class SameOwnerRequirement<TResource> : IAuthorizationRequirement 
        where TResource : IEntity, IOwnedEntity
    { }

    public static class ResourceAuthorizationExtensions
    {
        public static void AddPolicyAndHandler<TResource>(this IServiceCollection services, AuthorizationOptions options)
            where TResource : IEntity, IOwnedEntity
        {
            var resourceName = typeof(TResource).Name;
            options.AddPolicy(AuthPolicies.Edit + resourceName, policy =>
                policy.Requirements.Add(new SameOwnerRequirement<TResource>()));

            //services.AddSingleton<IAuthorizationHandler, ResourceAuthorizationHandler<TResource>>();
        }
    }
}
