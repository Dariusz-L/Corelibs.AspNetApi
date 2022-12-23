using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace Corelibs.AspNetApi.ModelBinders
{
    public abstract class FromRouteBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var properties = bindingContext.ModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
            var routeProperties = properties.Where(p => p.GetCustomAttributes().FirstOrDefault(a => a.GetType().Name == "FromRouteAttribute") != null).ToArray();

            var model = await GetModel(bindingContext, properties);
            if (model == null)
                model = Activator.CreateInstance(bindingContext.ModelType);

            var routeValues = bindingContext.HttpContext.GetRouteData().Values;
            foreach (var routeProperty in routeProperties) 
            {
                if (!routeValues.TryGetValue(routeProperty.Name, out var routeValue))
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                routeProperty.SetValue(model, routeValue);
            }

            bindingContext.Result = ModelBindingResult.Success(model);
        }

        protected abstract Task<object> GetModel(ModelBindingContext bindingContext, PropertyInfo[] properties);
    }
}
