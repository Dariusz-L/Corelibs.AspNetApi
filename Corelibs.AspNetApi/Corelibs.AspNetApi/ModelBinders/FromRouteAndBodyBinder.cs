using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Reflection;

namespace Corelibs.AspNetApi.ModelBinders
{
    public class FromRouteAndBodyBinder : FromRouteBinder
    {
        protected override async Task<object> GetModel(ModelBindingContext bindingContext, PropertyInfo[] properties)
        {
            String valueFromBody;
            using (var streamReader = new StreamReader(bindingContext.HttpContext.Request.Body))
                valueFromBody = await streamReader.ReadToEndAsync();

            var modelType = bindingContext.ModelMetadata.UnderlyingOrModelType;
            var result = JsonConvert.DeserializeObject(valueFromBody, modelType);

            return result;
        }
    }
}
