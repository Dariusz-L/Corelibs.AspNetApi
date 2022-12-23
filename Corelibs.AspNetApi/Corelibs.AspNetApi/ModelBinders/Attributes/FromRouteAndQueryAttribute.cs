using Microsoft.AspNetCore.Mvc;

namespace Corelibs.AspNetApi.ModelBinders
{
    public class FromRouteAndQueryAttribute : ModelBinderAttribute
    {
        public FromRouteAndQueryAttribute() : base(typeof(FromRouteAndQueryBinder)) { }
    }
}
