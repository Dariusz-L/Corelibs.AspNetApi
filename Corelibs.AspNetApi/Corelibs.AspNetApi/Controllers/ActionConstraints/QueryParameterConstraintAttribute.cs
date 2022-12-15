using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Corelibs.AspNetApi.Controllers.ActionConstraints
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class QueryParameterConstraintAttribute : ActionMethodSelectorAttribute
    {
        private readonly string _parameterName;
        private readonly string _parameterValue;

        public QueryParameterConstraintAttribute(string parameterName, string parameterValue)
        {
            _parameterName = parameterName;
            _parameterValue = parameterValue;
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            StringValues value;

            if (!routeContext.HttpContext.Request.Query.TryGetValue(_parameterName, out value))
                return false;

            return _parameterValue == value;
        }
    }
}
