using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;

namespace Corelibs.AspNetApi.Http
{
    public static class HttpContextExtensions
    {
        public static T GetEndpointMetadata<T>(this HttpContext httpContext)
            where T : class
        {
            return httpContext
              .Features
              .Get<IEndpointFeature>()
              .Endpoint
              .Metadata
              .GetMetadata<T>();
        }
    }
}
