using Common.Basic.Blocks;
using Common.Basic.Collections;
using Common.Basic.DDD;
using Common.Basic.Repository;
using Corelibs.AspNetApi.Http;
using Corelibs.Basic.Architecture.DDD;
using Corelibs.Basic.Collections;
using Corelibs.Basic.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Corelibs.AspNetApi.Authorization
{
    public class ResourceAuthorizationHandler : BaseAuthorizationHandler<SameOwnerRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public ResourceAuthorizationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<bool> HandleRequirementImplAsync(
            AuthorizationHandlerContext context,
            SameOwnerRequirement requirement)
        {
            var userID = context.User.GetUserID();
            if (userID.IsNullOrEmpty())
                return false;
            
            if (context.Resource is not HttpContext httpContext)
                return false;

            var editAttribute = httpContext.GetEndpointMetadata<Authorize_EditAttribute>();
            if (editAttribute ==  null) 
                return false;

            var actionDescriptor = httpContext.GetEndpointMetadata<ControllerActionDescriptor>();
            var requestParameter = actionDescriptor.MethodInfo.GetParameters().FirstOrDefault();
            if (requestParameter == null)
                return false;

            var properties = requestParameter.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var authorizedProperties = properties
                .Select(p => new 
                { 
                    property = p, 
                    authAttribute = p.GetCustomAttribute(typeof(AuthorizeResourceAttribute)) as AuthorizeResourceAttribute, 
                    fromRouteAttribute = p.GetCustomAttribute(typeof(FromRouteAttribute)) 
                })
                .Where(p => p.authAttribute != null)
                .ToArray();

            using (var scope = _serviceProvider.CreateScope())
            {
                var routeValues = httpContext.GetRouteData().Values;
                var bodyValues = await GetRequestJsonAsDictionary(httpContext);

                foreach (var p in authorizedProperties)
                {
                    var value = new KeyValuePair<string, object>();

                    var propertyName = p.property.Name.PascalToKebabCase();
                    if (p.fromRouteAttribute != null)
                    {
                        value = routeValues.FirstOrDefault(kv =>
                        {
                            var key = kv.Key.PascalToKebabCase();
                            return key == propertyName;
                        });
                    }

                    if (value.Key.IsNullOrEmpty())
                    {
                        value = bodyValues.FirstOrDefault(kv =>
                        {
                            var key = kv.Key.PascalToKebabCase();
                            return key == propertyName;
                        });
                    }

                    if (value.Key.IsNullOrEmpty() || (value.Value as string).IsNullOrEmpty())
                        return false;

                    var resourceID = value.Value as string;
                    if (!resourceID.IsID())
                        return false;

                    var repository = GetRepository(scope.ServiceProvider, p.authAttribute.ResourceType);

                    var result = new Result();
                    await repository.GetBy(resourceID, result);
                    if (!result.ValidateSuccessAndValues())
                        return false;

                   var resource = result.GetNestedValue<IOwnedEntity>();
                   if (resource == null)
                       return false;

                    if (userID != resource.OwnerID)
                        return false;
                }

                return true;
            }
        }

        public async Task<IDictionary<string, object>> GetRequestJsonAsDictionary(HttpContext context)
        {
            context.Request.Body.Position = 0;

            using (StreamReader reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                string bodyString = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(bodyString);
            }
        }

        public IReadRepository GetRepository(IServiceProvider serviceProvider, Type type)
        {
            var repositoryType = typeof(IRepository<>);
            var specificRepositoryType = repositoryType.MakeGenericType(type);
            
            var foundRepository = serviceProvider.GetRequiredService(specificRepositoryType);
            return foundRepository as IReadRepository;
        }
    }

    public class SameOwnerRequirement : IAuthorizationRequirement {}
}
