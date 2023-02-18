using Microsoft.AspNetCore.Authorization;

namespace Corelibs.AspNetApi.Authorization
{
    public static class AuthPolicies
    {
        public const string Edit = "EditPolicy";
    }

    public class Authorize_EditAttribute : AuthorizeAttribute 
    {
        public Authorize_EditAttribute() : base(AuthPolicies.Edit) {}
    }
}
