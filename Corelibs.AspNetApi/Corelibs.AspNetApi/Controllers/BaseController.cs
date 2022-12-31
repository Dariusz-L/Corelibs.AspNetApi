using Corelibs.AspNetApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Corelibs.AspNetApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        public string UserID => User.GetUserID();
    }
}
