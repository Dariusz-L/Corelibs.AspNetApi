namespace Corelibs.AspNetApi.Controllers.ActionConstraints
{
    public class ActionAttribute : QueryParameterConstraintAttribute
    {
        public ActionAttribute(string parameterValue) : base("action", parameterValue) { }
    }
}
