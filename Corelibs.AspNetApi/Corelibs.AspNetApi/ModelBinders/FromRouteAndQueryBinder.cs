using Common.Basic.Functional;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Reflection;
using System.Web;

namespace Corelibs.AspNetApi.ModelBinders
{
    public class FromRouteAndQueryBinder : FromRouteBinder
    {
        protected override Task<object> GetModel(ModelBindingContext bindingContext, PropertyInfo[] properties)
        {
            var queryCollection = HttpUtility.ParseQueryString(bindingContext.HttpContext.Request.QueryString.Value);
            if (queryCollection.IsNull() || queryCollection.Count == 0)
                return Task.FromResult<object>(null);

            string json = JsonConvert.SerializeObject(queryCollection.Cast<string>().ToDictionary(k => k, v => queryCollection[v]));
            var model = JsonConvert.DeserializeObject(json, bindingContext.ModelType);

            return Task.FromResult(model);
        }
    }
}
