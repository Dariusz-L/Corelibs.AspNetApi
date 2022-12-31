using Common.Basic.Repository;
using Corelibs.Basic.Architecture.DDD;
using Microsoft.AspNetCore.Http;

namespace Corelibs.AspNetApi.Authorization
{
    public class CurrentUserAccessor : IAccessor<CurrentUser>
    {
        private readonly HttpContext _httpContext;

        public CurrentUserAccessor(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public CurrentUser Get()
        {
            var userID = _httpContext.User.GetUserID();
            return new CurrentUser(userID);
        }
    }
}
