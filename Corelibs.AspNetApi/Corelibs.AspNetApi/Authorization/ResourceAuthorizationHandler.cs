using Common.Basic.Blocks;
using Common.Basic.Collections;
using Common.Basic.DDD;
using Common.Basic.Repository;
using Corelibs.Basic.Architecture.DDD;
using Corelibs.Basic.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Corelibs.AspNetApi.Authorization
{
    public class ResourceAuthorizationHandler<TResource> : BaseAuthorizationHandler<SameOwnerRequirement<TResource>>
        where TResource : IEntity, IOwnedEntity
    {
        private readonly IServiceProvider _serviceProvider;

        public ResourceAuthorizationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<bool> HandleRequirementImplAsync(
            AuthorizationHandlerContext context,
            SameOwnerRequirement<TResource> requirement)
        {
            var userID = context.User.GetUserID();
            if (userID.IsNullOrEmpty())
                return false;
            
            if (context.Resource is not HttpContext httpContext)
                return false;

            using (var scope = _serviceProvider.CreateScope())
            {
                var resourceRespository = scope.ServiceProvider.GetRequiredService<IRepository<TResource>>();

                var routeValues = httpContext.GetRouteData().Values;
                var routeValue = routeValues.LastOrDefault(kv => kv.Key.ToLower().Contains("id"));
                if (routeValue.Key.IsNullOrEmpty() || (routeValue.Value as string).IsNullOrEmpty())
                    return false;

                var resourceID = routeValue.Value as string;
                if (!resourceID.IsID())
                    return false;

                var result = new Result();
                var resource = await resourceRespository.Get(resourceID, result);
                if (resource == null)
                    return false;

                if (userID != resource.OwnerID)
                    return false;

                return true;
            }
        }
    }

    public class SameOwnerRequirement<TResource> : IAuthorizationRequirement 
        where TResource : IEntity, IOwnedEntity
    {}

    public static class ResourceAuthorizationExtensions
    {
        public static void AddPolicyAndHandler<TResource>(this IServiceCollection services, AuthorizationOptions options)
            where TResource : IEntity, IOwnedEntity
        {
            var resourceName = typeof(TResource).Name;
            options.AddPolicy(AuthPolicies.Edit + resourceName, policy =>
                policy.Requirements.Add(new SameOwnerRequirement<TResource>()));
        }
    }
}
