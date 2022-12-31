using Common.Basic.Repository;
using Corelibs.Basic.Architecture.DDD;
using Microsoft.AspNetCore.Http;

namespace Corelibs.AspNetApi.Authorization
{
    public class CurrentUserAccessor : IAccessor<CurrentUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CurrentUser Get()
        {
            var userID = _httpContextAccessor.HttpContext.User.GetUserID();
            return new CurrentUser(userID);
        }
    }
}
