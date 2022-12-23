using Microsoft.AspNetCore.Mvc;

namespace Corelibs.AspNetApi.ModelBinders
{
    public class FromRouteAndBodyAttribute : ModelBinderAttribute
    {
        public FromRouteAndBodyAttribute() : base(typeof(FromRouteAndBodyBinder)) {}
    }
}
